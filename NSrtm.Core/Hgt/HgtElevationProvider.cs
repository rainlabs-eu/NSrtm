using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    public class HgtElevationProvider : IElevationProvider
    {
        private readonly IHgtDataCellFactory _cellFactory;
        private readonly ConcurrentDictionary<HgtCellCoords, IHgtDataCell> _cache = new ConcurrentDictionary<HgtCellCoords, IHgtDataCell>();

        internal HgtElevationProvider([NotNull] IHgtDataCellFactory cellFactory)
        {
            if (cellFactory == null) throw new ArgumentNullException("cellFactory");
            _cellFactory = cellFactory;
            Name = "Unknown";
            Description = "Unknown";
        }

        [NotNull] public string Name { get; set; }
        [NotNull] public string Description { get; set; }

        public double GetElevation(double latitude, double longitude)
        {
            var coords = HgtCellCoords.ForLatLon(latitude, longitude);

            var cell = _cache.GetOrAdd(coords, buildCellFor);

            return cell.GetElevation(latitude, longitude);
        }

        [NotNull]
        private IHgtDataCell buildCellFor(HgtCellCoords coords)
        {
            try
            {
                return _cellFactory.GetCellFor(coords);
            }
            catch (HgtFileException)
            {
                return HgtDataCellInvalid.Invalid;
            }
        }

        [NotNull]
        public static IElevationProvider CreateInMemoryFromRawFiles([NotNull] string directory)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverRaw();
            IHgtDataLoader loader = new HgtDataLoaderFromRaw(pathResolver);
            return new HgtElevationProvider(new HgtDataCellInMemoryFactory(directory, loader))
                   {
                       Name = "SRTM files",
                       Description = string.Format("Unpacked SRTM files (HGT) from directory {0}", directory)
                   };
        }

        [NotNull]
        public static IElevationProvider CreateInMemoryFromZipFiles([NotNull] string directory)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverZip();
            IHgtDataLoader loader = new HgtDataLoaderFromZip(pathResolver);
            return new HgtElevationProvider(new HgtDataCellInMemoryFactory(directory, loader))
                   {
                       Name = "SRTM files",
                       Description = string.Format("ZIP packed SRTM files (HGT.ZIP) from directory {0}", directory)
                   };
        }

        [NotNull]
        public static IElevationProvider CreateMemoryMappedFromRawFiles([NotNull] string directory)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverRaw();

            return new HgtElevationProvider(new HgtDataCellInFileFactory(directory, pathResolver))
                   {
                       Name = "SRTM files",
                       Description = string.Format("Memory mapped SRTM files (HGT) from directory {0}", directory)
                   };
        }
    }
}
