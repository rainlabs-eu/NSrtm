using System;
using System.Collections.Concurrent;
using System.Linq;
using JetBrains.Annotations;
using NSrtm.Core.BicubicInterpolation;
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

        internal BivariatePolynomial interpolateCellSurface(PgmCellCoords pgmCellCoords)
        {
            var latitudes = Enumerable.Range(-1, 4)
                                      .Select(step => pgmCellCoords.Lon + step * _discreteSurface.PgmParameters.LongitudeIncrementDegrees);
            var longitudes = Enumerable.Range(-1, 4)
                                       .Select(step => pgmCellCoords.Lat + step * _discreteSurface.PgmParameters.LatitudeIncrementDegrees);
            var coordinates = latitudes.SelectMany(lat => longitudes.Select(lon => normalizeCoords(lat,lon))).ToList();
            var data = coordinates.Select(coor => _discreteSurface.GetUndulation(coor.Lat, coor.Lon));
            var dataFormated = data.Select((c, i) => new { Index = i, value = c })
                            .GroupBy(p => p.Index / 4)
                            .Select(c => c.Select(v => v.value)
                                          .ToList())
                            .ToList();
            var spline = BicubicCalculator.GetSpline(dataFormated, _discreteSurface.PgmParameters.LatitudeIncrementDegrees);
            return spline;
        }

        private static PgmCellCoords normalizeCoords(double lat, double lon)
        {
            var pushedLon = lon;
            if (lat > 90 || lat < -90)
            {
                pushedLon += 180;
            }
            return new PgmCellCoords(normalizeLatitude(lat), normalizeLongitude(pushedLon));
        }

        private static double normalizeLongitude(double lon)
        {
            return (360 + (lon % 360)) % 360;
        }

        private static double normalizeLatitude(double lat)
        {
            return Math.Atan(Math.Sin(lat) / Math.Abs(Math.Cos(lat)));
        }

        public double GetElevation(double latitude, double longitude)
        {
            var mainNode = PgmCellCoordsExtensions.FromLatLon(latitude, longitude, _discreteSurface.PgmParameters);
            var interpolatedCell = _continuousSurface.GetOrAdd(mainNode, interpolateCellSurface);
            return interpolatedCell.Evaluate(latitude, longitude);
        }

        public Level ElevationBase { get { return _discreteSurface.PgmParameters.Level; } }
        public Level ElevationTarget { get { return Level.EllipsoidWgs84; } }
    }
}
