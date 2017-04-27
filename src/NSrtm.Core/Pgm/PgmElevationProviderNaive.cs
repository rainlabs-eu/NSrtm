using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NSrtm.Core.BicubicInterpolation;
using NSrtm.Core.Pgm.DataDesciption;
using NSrtm.Core.Pgm.GeoidUndulationGrid;

namespace NSrtm.Core.Pgm
{
    public sealed class PgmElevationProviderNaive : IElevationProvider
    {
        private readonly ConcurrentDictionary<PgmCellCoords, double> _elevationCache =
            new ConcurrentDictionary<PgmCellCoords, double>();

        private readonly IPgmGeoidUndulationGrid _undulationGrid;

        internal PgmElevationProviderNaive([NotNull] IPgmGeoidUndulationGrid undulationGrid)
        {
            if (undulationGrid == null) throw new ArgumentNullException(nameof(undulationGrid));
            _undulationGrid = undulationGrid;
        }

        private PgmDataDescription dataDescription => _undulationGrid.PgmParameters;

        /// <summary>
        ///     Gets elevation of the geoid above the ellipsoid.
        ///     Does not use any interpolation.
        /// </summary>
        /// <param name="latitude">Latitude in degrees in WGS84 datum</param>
        /// <param name="longitude">Longitude in degrees in WGS84 datum</param>
        /// <returns></returns>
        public double GetElevation(double latitude, double longitude)
        {
            CoordsValidator.ThrowIfNotValidWgs84(latitude, longitude);

            var cellCoords = PgmCellCoords.ForCoordinatesUsingDescription(latitude, longitude, dataDescription);
            return _elevationCache.GetOrAdd(cellCoords, coords => _undulationGrid.GetUndulation(coords.Lat, coords.Lon));         
        }

        public Task<double> GetElevationAsync(double latitude, double longitude)
        {
            return Task.FromResult(GetElevation(latitude, longitude));
        }

        public Level ElevationBase => Level.EllipsoidWgs84;

        public Level ElevationTarget => dataDescription.Level;       

        #region Provider creation

        /// <summary>
        ///     Creates elevation provider which loads PGM file to memory and uses bicubic spline interpolation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static PgmElevationProviderNaive CreateWithGridDataStoredInMemory([NotNull] string path)
        {
            var pgmGeoidUndulationMemory = PgmGeoidUndulationGridFactory.CreateGeoidUndulationGridInMemory(path);
            return new PgmElevationProviderNaive(pgmGeoidUndulationMemory);
        }

        /// <summary>
        ///     Creates elevation provider which reads PGM data from file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static PgmElevationProviderNaive CreateWithDirectAccessToGridData([NotNull] string path)
        {
            var pgmGeoidUndulationFile = PgmGeoidUndulationGridFactory.CreateGeoidUndulationGridInFile(path);
            return new PgmElevationProviderNaive(pgmGeoidUndulationFile);
        }

        #endregion Provider creation
    }
}