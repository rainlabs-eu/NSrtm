using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using NSrtm.Core.BicubicInterpolation;
using NSrtm.Core.Pgm.GeoidUndulationGrid;

namespace NSrtm.Core.Pgm
{
    public sealed class PgmElevationProvider
    {
        private readonly IPgmGeoidUndulationGrid _discreteSurface;

        private readonly ConcurrentDictionary<PgmCellCords, BivariatePolynomial> _continuousSurface =
            new ConcurrentDictionary<PgmCellCords, BivariatePolynomial>();

        public PgmElevationProvider([NotNull] IPgmGeoidUndulationGrid discreteSurface)
        {
            if (discreteSurface == null) throw new ArgumentNullException("discreteSurface");
            _discreteSurface = discreteSurface;
        }

        public double GetElevation(double latitude, double longitude)
        {
            var mainNode = PgmCellCoordsExtensions.FromLatLon(latitude, longitude, _discreteSurface.PgmParameters);
            var interpolatedCell = _continuousSurface.GetOrAdd(mainNode, interpolateCellSurface);
            return interpolatedCell.Evaluate(latitude, longitude);
        }

        private BivariatePolynomial interpolateCellSurface(PgmCellCords pgmCellCords)
        {
            throw new NotImplementedException();
        }
    }
}
