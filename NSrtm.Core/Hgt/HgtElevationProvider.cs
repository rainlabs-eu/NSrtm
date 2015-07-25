using System;
using System.Runtime.Caching;

namespace NSrtm.Core
{
    public class HgtElevationProvider : IElevationProvider
    {
        private readonly IHgtDataCellFactory _cellFactory;
        private readonly CacheItemPolicy _policy;
        private readonly ObjectCache _cache;

        public HgtElevationProvider(IHgtDataCellFactory cellFactory, ObjectCache cache = null, CacheItemPolicy policy = null)
        {
            if (cellFactory == null) throw new ArgumentNullException("cellFactory");
            _cellFactory = cellFactory;
            _policy = policy ?? new CacheItemPolicy {SlidingExpiration = TimeSpan.FromSeconds(1)};

            _cache = cache ?? MemoryCache.Default;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public double GetElevation(double latitude, double longitude)
        {
            HgtCellCoords coords = HgtCellCoords.ForLatLon( latitude,  longitude);

            string baseName = coords.ToBaseName();
            var cell = _cache.Get(baseName) as IHgtDataCell;
            if (cell == null)
            {
                IHgtDataCell newCell;
                try
                {
                    newCell = _cellFactory.GetCellFor(coords);
                }
                catch (HgtFileNotFoundException)
                {
                    newCell = HgtDataCellInvalid.Invalid;
                }
                catch (Exception ex)
                {
                    throw new HgtFileException(coords, "Unknown error during cell retrieval", ex);
                }
                cell = _cache.AddOrGetExisting(baseName, newCell, _policy) as IHgtDataCell;
                cell = cell ?? newCell;
            }

            if (cell != null)
                return cell.GetElevation(latitude, longitude);
            else
                return Double.NaN;
        }

        public static HgtElevationProvider CreateInMemoryFromRawFiles(string directory, ObjectCache cache = null, CacheItemPolicy policy = null)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverRaw();
            IHgtDataLoader loader = new HgtDataLoaderFromRaw(pathResolver);
            return new HgtElevationProvider(new HgtDataCellInMemoryFactory(directory, loader), cache, policy)
                   {
                       Name = "SRTM files",
                       Description = string.Format("Unpacked SRTM files (HGT) from directory {0}", directory)
                   };
        }
        public static HgtElevationProvider CreateInMemoryFromZipFiles(string directory, ObjectCache cache = null, CacheItemPolicy policy = null)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverZip();
            IHgtDataLoader loader = new HgtDataLoaderFromZip(pathResolver);
            return new HgtElevationProvider(new HgtDataCellInMemoryFactory(directory, loader), cache, policy)
            {
                Name = "SRTM files",
                Description = string.Format("ZIP packed SRTM files (HGT.ZIP) from directory {0}", directory)
            };
        }
    }
}
