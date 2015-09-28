using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NSrtm.Core.BicubicInterpolation;
using NSrtm.Core.Pgm.DataDesciption;
using NSrtm.Core.Pgm.GeoidUndulationGrid;

namespace NSrtm.Core.Pgm
{
    internal sealed class PgmElevationProvider
    {
        private readonly IPgmGeoidUndulationGrid _discreteSurface;

        private readonly ConcurrentDictionary<PgmCellCoords, BivariatePolynomial> _continuousSurface =
            new ConcurrentDictionary<PgmCellCoords, BivariatePolynomial>();

        public PgmElevationProvider([NotNull] IPgmGeoidUndulationGrid discreteSurface)
        {
            if (discreteSurface == null) throw new ArgumentNullException("discreteSurface");
            _discreteSurface = discreteSurface;
        }

        private PgmDataDescription dataDescription { get { return _discreteSurface.PgmParameters; } }

        private BivariatePolynomial getInterpolatioForCellSurface(PgmCellCoords pgmCellCoords)
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
            return BicubicCalculator.GetSpline(values: formattedUndulations, step: dataDescription.LatitudeIncrementDegrees);
        }

        private static PgmCellCoords normalizeCoords(double lat, double lon)
        {
            var normalizedLongitude = lon;
            if (lat > 90 || lat < -90)
            {
                normalizedLongitude += 180;
            }
            normalizedLongitude = (360 + (normalizedLongitude % 360)) % 360;
            var absoluteLat = Math.Abs(Math.Abs(lat % 180) - 90);
            var normalizedLatitude = lat > 0 ? 90 - absoluteLat : absoluteLat - 90;
            return new PgmCellCoords(normalizedLatitude, normalizedLongitude);
        }

        /// <summary>
        ///     Gets elevation of the geoid above the ellipsoid.
        /// </summary>
        /// <param name="latitude">Latitude in degrees in WGS84 datum</param>
        /// <param name="longitude">Longitude in degrees in WGS84 datum</param>
        /// <returns></returns>
        public double GetElevation(double latitude, double longitude)
        {
            var longitudeEgmDatum = longitude + 180;
            var mainNode = PgmCellCoords.ForCoordinatesUsingDescription(latitude, longitudeEgmDatum, dataDescription);
            var interpolatedCell = _continuousSurface.GetOrAdd(mainNode, getInterpolatioForCellSurface);
            return interpolatedCell.Evaluate(latitude, longitudeEgmDatum);
        }

        public Level ElevationBase { get { return dataDescription.Level; } }
        public Level ElevationTarget { get { return Level.EllipsoidWgs84; } }

        #region Static Members

        internal static IEnumerable<PgmCellCoords> findCellAndSurroundingNodesCoords(PgmCellCoords pgmCellCoords, double latIncrement, double lonIncrement)
        {
            var horizontalNodes = Enumerable.Range(-1, 4)
                                            .Select(step => pgmCellCoords.Lon + step * lonIncrement);
            var verticalNodes = Enumerable.Range(-1, 4)
                                          .Select(step => pgmCellCoords.Lat + step * latIncrement);
            return verticalNodes.SelectMany(lat => horizontalNodes.Select(lon => normalizeCoords(lat, lon)))
                                  .ToList();
        }

        /// <summary>
        ///     Creates elevation provider which loads PGM file to memory and uses bicubic spline interpolation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static IElevationProvider CreateWithStoredGridInMemory([NotNull] string path)
        {
            var pgmGeoidUndulationMemory = PgmGeoidUndulationGridFactory.CreateGeoidUndulationGridInMemory(path);
            return new PgmElevationProvider(pgmGeoidUndulationMemory);
        }

        /// <summary>
        ///     Creates elevation provider which reads PGM data from file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Created provider.</returns>
        [NotNull]
        public static IElevationProvider CreateDirectAccessToGrid([NotNull] string path)
        {
            var pgmGeoidUndulationFile = PgmGeoidUndulationGridFactory.CreateGeoidUndulationGridInFile(path);
            return new PgmElevationProvider(pgmGeoidUndulationFile);
        }

        #endregion
    }
}
