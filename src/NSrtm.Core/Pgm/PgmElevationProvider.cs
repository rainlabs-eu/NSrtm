using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NSrtm.Core.BicubicInterpolation;
using NSrtm.Core.Pgm.GeoidUndulationGrid;

namespace NSrtm.Core.Pgm
{
    public sealed class PgmElevationProvider : IElevationProvider
    {
        private IPgmGeoidUndulationGrid _discreteSurface;
        private ConcurrentDictionary<PgmCellCords, BivariatePolynomial> _continuousSurface = new ConcurrentDictionary<PgmCellCords, BivariatePolynomial>();
        private Level _elevationBase;
        private Level _elevationTarget;

        public PgmElevationProvider([NotNull] IPgmGeoidUndulationGrid discreteSurface)
        {
            if (discreteSurface == null) throw new ArgumentNullException("discreteSurface");
            _discreteSurface = discreteSurface;
        }

        public double GetElevation(double latitude, double longitude)
        {
        ???
        }

        public Task<double> GetElevationAsync(double latitude, double longitude)
        {
        ???
        }

        public Level ElevationBase { get { return _elevationBase; } }

        public Level ElevationTarget { get { return _elevationTarget; } }
    }
}