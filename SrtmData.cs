#region MIT License

// MIT License
// Copyright (c) 2012 Alpine Chough Software.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.	

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace NSrtm
{
    /// <summary>
    ///     SRTM Data.
    /// </summary>
    public class SrtmData : IElevationProvider
    {
        private readonly string _dataDirectory;
        private readonly ObjectCache _cache;
        private readonly ConcurrentDictionary<Tuple<int, int>, string> _filePaths = new ConcurrentDictionary<Tuple<int, int>, string>();
        #region Lifecycle

        /// <summary>
        ///     Initializes a new instance of the <see cref="SrtmData" /> class.
        /// </summary>
        /// <param name='dataDirectory'>
        ///     Data directory.
        /// </param>
        /// <param name="cache">Optional argument to override MemoryCache.Default</param>
        /// <exception cref='DirectoryNotFoundException'>
        ///     Is thrown when part of a file or directory argument cannot be found.
        /// </exception>
        public SrtmData(string dataDirectory, ObjectCache cache = null)
        {
            if (dataDirectory == null) throw new ArgumentNullException("dataDirectory");

            if (!Directory.Exists(dataDirectory))
                throw new DirectoryNotFoundException(dataDirectory);

            _cache = cache ?? MemoryCache.Default;

            _dataDirectory = dataDirectory;
        }

        #endregion

        #region Public methods

        public double GetElevation(double latitude, double longitude)
        {
            int cellLatitude = (int)Math.Floor(Math.Abs(latitude));
            if (latitude < 0)
                cellLatitude *= -1;

            int cellLongitude = (int)Math.Floor(Math.Abs(longitude));
            if (longitude < 0)
                cellLongitude *= -1;

            var cellId = Tuple.Create(cellLatitude, cellLongitude);
            string filePath = _filePaths.GetOrAdd(cellId, tpl => buildFilePath(tpl.Item1, tpl.Item2));

            var cell = _cache.Get(filePath) as SrtmDataCell;
            if (cell == null)
            {
                cell = new SrtmDataCell(filePath);
                var cacheItemPolicy = new CacheItemPolicy
                                      {
                                          ChangeMonitors = {new HostFileChangeMonitor(new[] {filePath})}
                                      };
                cell = _cache.AddOrGetExisting(filePath, cell, cacheItemPolicy) as SrtmDataCell;
            }

            if (cell != null)
                return cell.GetHeight(latitude, longitude);
            else
                throw new NotImplementedException();
        }

        private string buildFilePath(int cellLatitude, int cellLongitude)
        {
            string filename = string.Format("{0}{1:D2}{2}{3:D3}.hgt",
                                            cellLatitude < 0 ? "S" : "N",
                                            Math.Abs(cellLatitude),
                                            cellLongitude < 0 ? "W" : "E",
                                            Math.Abs(cellLongitude));

            string filePath = Path.Combine(_dataDirectory, filename);
            return filePath;
        }
        #endregion
    }
}
