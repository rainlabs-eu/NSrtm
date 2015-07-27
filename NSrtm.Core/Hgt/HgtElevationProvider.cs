using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    /// <summary>
    /// Provides elevation from SRTM3 and SRTM1 format files (HGT or HGT.ZIP) downloaded from:
    /// http://dds.cr.usgs.gov/srtm/version2_1/
    /// </summary>
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

        /// <summary>
        ///     Short name describing implementation. Used for UI/Demos where different implementations are available.
        /// </summary>
        [NotNull] public string Name { get; set; }

        /// <summary>
        ///     More descriptive info about implementation. Used for UI/Demos where different implementations are available.
        /// </summary>
        [NotNull] public string Description { get; set; }

        /// <summary>
        ///     Gets elevation above MSL
        /// </summary>
        /// <param name="latitude">Latitude in degrees in WGS84 datum</param>
        /// <param name="longitude">Longitude in degrees in WGS84 datum</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates elevation provider which loads HGT files to memory on demand.
        /// </summary>
        /// <remarks>Files are loaded and cached forever, so for "whole world coverage" it can use up to 16GB of RAM.</remarks>
        /// <param name="directory"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static IElevationProvider CreateInMemoryFromRawFiles([NotNull] string directory)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverRaw(directory);
            IHgtDataLoader loader = new HgtDataLoaderFromRaw(pathResolver);
            return new HgtElevationProvider(new HgtDataCellInMemoryFactory(loader))
                   {
                       Name = "SRTM files",
                       Description = string.Format("Unpacked SRTM files (HGT) from directory {0}", directory)
                   };
        }

        /// <summary>
        /// Creates elevation provider which loads HGT.ZIP files to memory on demand.
        /// </summary>
        /// <remarks>Files are loaded and cached forever, so for "whole world coverage" it can use up to 16GB of RAM.</remarks>
        /// <param name="directory"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static IElevationProvider CreateInMemoryFromZipFiles([NotNull] string directory)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverZip(directory);
            IHgtDataLoader loader = new HgtDataLoaderFromZip(pathResolver);
            return new HgtElevationProvider(new HgtDataCellInMemoryFactory(loader))
                   {
                       Name = "SRTM files",
                       Description = string.Format("ZIP packed SRTM files (HGT.ZIP) from directory {0}", directory)
                   };
        }

        /// <summary>
        /// Creates elevation provider which reads HGT files from disk.
        /// </summary>
        /// <remarks>It is 1000 - 10000 times slower than in memory implementations, but it uses almost no RAM.</remarks>
        /// <param name="directory"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static IElevationProvider CreateDirectDiskAccessFromRawFiles([NotNull] string directory)
        {
            IHgtPathResolver pathResolver = new HgtPathResolverRaw(directory);
            return new HgtElevationProvider(new HgtDataCellInFileFactory(pathResolver))
                   {
                       Name = "SRTM files",
                       Description = string.Format("Memory mapped SRTM files (HGT) from directory {0}", directory)
                   };
        }
    }
}
