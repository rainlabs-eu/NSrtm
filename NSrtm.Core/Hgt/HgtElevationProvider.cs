using System;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    public class HgtElevationProvider : IElevationProvider
    {
        private readonly IHgtDataCellFactory _cellFactory;

        public HgtElevationProvider([NotNull] IHgtDataCellFactory cellFactory)
        {
            if (cellFactory == null) throw new ArgumentNullException("cellFactory");
            _cellFactory = cellFactory;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public double GetElevation(double latitude, double longitude)
        {
            HgtCellCoords coords = HgtCellCoords.ForLatLon(latitude, longitude);

            var cell = _cellFactory.GetCellFor(coords);

            return cell.GetElevation(latitude, longitude);
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

            return new HgtElevationProvider(new HgtDataCellInMemoryMappedFactory(directory, pathResolver))
                   {
                       Name = "SRTM files",
                       Description = string.Format("Memory mapped SRTM files (HGT) from directory {0}", directory)
                   };
        }
    }
}
