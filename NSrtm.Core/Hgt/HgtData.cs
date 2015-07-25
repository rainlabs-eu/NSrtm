using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Runtime.Caching;

namespace NSrtm.Core
{
    /// <summary>
    ///     SRTM Data.
    /// </summary>
    public class HgtData : IElevationProvider
    {
        private readonly string _dataDirectory;
        private readonly ObjectCache _cache;
        private readonly ConcurrentDictionary<Tuple<int, int>, string> _filePaths = new ConcurrentDictionary<Tuple<int, int>, string>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="HgtData" /> class.
        /// </summary>
        /// <param name='dataDirectory'>
        ///     Data directory.
        /// </param>
        /// <param name="cache">Optional argument to override MemoryCache.Default</param>
        /// <exception cref='DirectoryNotFoundException'>
        ///     Is thrown when part of a file or directory argument cannot be found.
        /// </exception>
        public HgtData(string dataDirectory, ObjectCache cache = null)
        {
            if (dataDirectory == null) throw new ArgumentNullException("dataDirectory");

            if (!Directory.Exists(dataDirectory))
                throw new DirectoryNotFoundException(dataDirectory);

            _cache = cache ?? MemoryCache.Default;

            _dataDirectory = dataDirectory;
        }

        public double GetElevation(double latitude, double longitude)
        {
            int cellLatitude = (int)Math.Floor(Math.Abs(latitude));
            if (latitude < 0)
                cellLatitude *= -1;

            int cellLongitude = (int)Math.Floor(longitude);

            var cellId = Tuple.Create(cellLatitude, cellLongitude);
            string filePath = _filePaths.GetOrAdd(cellId, tpl => buildFilePath(tpl.Item1, tpl.Item2));

            var cell = _cache.Get(filePath) as IHgtDataCell;
            if (cell == null)
            {
                var newCell = HgtDataCell.FromZipFileOrInvalid(filePath);
                var cacheItemPolicy = new CacheItemPolicy
                                      {
                                          ChangeMonitors = {new HostFileChangeMonitor(new[] {filePath})}
                                      };
                cell = _cache.AddOrGetExisting(filePath, newCell, cacheItemPolicy) as IHgtDataCell;
                cell = cell ?? newCell;
            }

            if (cell != null)
                return cell.GetElevation(latitude, longitude);
            else
                throw new NotImplementedException();
        }

        private string buildFilePath(int cellLatitude, int cellLongitude)
        {
            string filename = string.Format(CultureInfo.InvariantCulture,
                                            "{0}{1:D2}{2}{3:D3}.hgt.zip",
                                            cellLatitude < 0 ? "S" : "N",
                                            Math.Abs(cellLatitude),
                                            cellLongitude < 0 ? "W" : "E",
                                            Math.Abs(cellLongitude));

            string filePath = Path.Combine(_dataDirectory, filename);
            return filePath;
        }
    }
}
