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
    public sealed class PgmElevationProviderBicubic : IElevationProvider
    {
        private readonly IPgmGeoidUndulationGrid _discreteSurface;

        private readonly ConcurrentDictionary<PgmCellCoords, BivariatePolynomial> _continuousSurface =
            new ConcurrentDictionary<PgmCellCoords, BivariatePolynomial>();

        internal PgmElevationProviderBicubic([NotNull] IPgmGeoidUndulationGrid discreteSurface)
        {
            if (discreteSurface == null) throw new ArgumentNullException("discreteSurface");
            _discreteSurface = discreteSurface;
        }

        private PgmDataDescription dataDescription { get { return _discreteSurface.PgmParameters; } }

        private BivariatePolynomial getInterpolationForCellSurface(PgmCellCoords pgmCellCoords)
        {
            var nodesCoordinates = findCellAndSurroundingNodesCoords(pgmCellCoords,
                                                                     dataDescription.LatitudeIncrementDegrees,
                                                                     dataDescription.LongitudeIncrementDegrees);
            var nodesUndulations = nodesCoordinates.Select(coor => _discreteSurface.GetUndulation(coor.Lat, coor.Lon));
            var formattedUndulations = nodesUndulations.Select((c, i) => new {Index = i, value = c})
                                                       .GroupBy(p => p.Index / 4)
                                                       .Select(c => c.Select(v => v.value)
                                                                     .ToList())
                                                       .ToList();
            return BicubicCalculator.GetSpline(values: formattedUndulations);
        }

        internal static PgmCellCoords normalizeCoords(double lat, double lon)
        {
            if (lat > 90 || lat < -90)
            {
                var lonOnOtherSide = lon + 180;
                var normalizedLongitude = (360 + (lonOnOtherSide % 360)) % 360;
                var absoluteLat = Math.Abs(Math.Abs(lat % 180) - 90);
                var normalizedLatitude = lat > 0 ? 90 - absoluteLat : absoluteLat - 90;
                return new PgmCellCoords(normalizedLatitude, normalizedLongitude);
            }
            else
            {
                var normalizedLongitude = (360 + (lon % 360)) % 360;
                return new PgmCellCoords(lat, normalizedLongitude);
            }
        }

        /// <summary>
        ///     Gets elevation of the geoid above the ellipsoid.
        /// </summary>
        /// <param name="latitude">Latitude in degrees in WGS84 datum</param>
        /// <param name="longitude">Longitude in degrees in WGS84 datum</param>
        /// <returns></returns>
        public double GetElevation(double latitude, double longitude)
        {
            CoordsValidator.ThrowIfNotValidWgs84(latitude, longitude);

            var cellCoords = PgmCellCoords.ForCoordinatesUsingDescription(latitude, longitude, dataDescription);
            var interpolatedCell = _continuousSurface.GetOrAdd(cellCoords, getInterpolationForCellSurface);
            var coordsWithinCell = calculateUnitCoordsWithinCell(cellCoords, latitude, longitude);
            return interpolatedCell.Evaluate(coordsWithinCell.Item1, coordsWithinCell.Item2);
        }

        private Tuple<double, double> calculateUnitCoordsWithinCell(PgmCellCoords cellCoords, double latitude, double longitude)
        {
            double offsetLat = latitude - cellCoords.Lat;
            double offsetLon = longitude - cellCoords.Lon;

            double scaledOffsetLat = offsetLat / dataDescription.LatitudeIncrementDegrees;
            double scaledOffsetLon = offsetLon / dataDescription.LongitudeIncrementDegrees;

            return Tuple.Create(scaledOffsetLat, scaledOffsetLon);
        }

        public Task<double> GetElevationAsync(double latitude, double longitude)
        {
            return Task.FromResult(GetElevation(latitude, longitude));
        }

        public Level ElevationBase { get { return Level.EllipsoidWgs84; } }
        public Level ElevationTarget { get { return dataDescription.Level; } }

        #region Static Members

        internal static IEnumerable<PgmCellCoords> findCellAndSurroundingNodesCoords(PgmCellCoords pgmCellCoords, double latIncrement, double lonIncrement)
        {
            IEnumerable<double> horizontalNodes = Enumerable.Range(-1, 4)
                                                            .Select(step => pgmCellCoords.Lon + step * lonIncrement);
            IEnumerable<double> verticalNodes = Enumerable.Range(-1, 4)
                                                          .Select(step => pgmCellCoords.Lat + step * latIncrement);
            return verticalNodes.SelectMany(lat => horizontalNodes.Select(lon => normalizeCoords(lat, lon)))
                                .ToList();
        }

        #endregion

        #region Provider creation

        /// <summary>
        ///     Creates elevation provider which loads PGM file to memory and uses bicubic spline interpolation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static IElevationProvider CreateWithGridDataStoredInMemory([NotNull] string path)
        {
            var pgmGeoidUndulationMemory = PgmGeoidUndulationGridFactory.CreateGeoidUndulationGridInMemory(path);
            return new PgmElevationProviderBicubic(pgmGeoidUndulationMemory);
        }

        /// <summary>
        ///     Creates elevation provider which reads PGM data from file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static IElevationProvider CreateWithDirectAccessToGridData([NotNull] string path)
        {
            var pgmGeoidUndulationFile = PgmGeoidUndulationGridFactory.CreateGeoidUndulationGridInFile(path);
            return new PgmElevationProviderBicubic(pgmGeoidUndulationFile);
        }

        #endregion Provider creation
    }
}
