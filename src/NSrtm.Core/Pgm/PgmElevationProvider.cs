using System;
using System.Collections.Concurrent;
using System.Linq;
using JetBrains.Annotations;
using NSrtm.Core.BicubicInterpolation;
using NSrtm.Core.Pgm.DataDesciption;
using NSrtm.Core.Pgm.GeoidUndulationGrid;

namespace NSrtm.Core.Pgm
{
    public sealed class PgmElevationProvider
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

        internal BivariatePolynomial getInterpolatioForCellSurface(PgmCellCoords pgmCellCoords)
        {
            var horizontalNodes = Enumerable.Range(-1, 4)
                                            .Select(step => pgmCellCoords.Lon + step * dataDescription.LongitudeIncrementDegrees);
            var verticalNodes = Enumerable.Range(-1, 4)
                                          .Select(step => pgmCellCoords.Lat + step * dataDescription.LatitudeIncrementDegrees);
            var nodesCoordinates = horizontalNodes.SelectMany(lat => verticalNodes.Select(lon => normalizeCoords(lat, lon)))
                                                  .ToList();
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
            var normalizedLatitude = Math.Atan(Math.Sin(lat) / Math.Abs(Math.Cos(lat)));
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
            var mainNode = PgmCellCoords.ForCoordinatesUsingDescription(latitude, longitudeEgmDatum, _discreteSurface.PgmParameters);
            var interpolatedCell = _continuousSurface.GetOrAdd(mainNode, getInterpolatioForCellSurface);
            return interpolatedCell.Evaluate(latitude, longitudeEgmDatum);
        }

        public Level ElevationBase { get { return _discreteSurface.PgmParameters.Level; } }
        public Level ElevationTarget { get { return Level.EllipsoidWgs84; } }
    }
}
