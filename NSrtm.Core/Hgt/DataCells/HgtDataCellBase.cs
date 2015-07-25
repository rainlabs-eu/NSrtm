using System;

namespace NSrtm.Core
{
    internal abstract class HgtDataCellBase : IHgtDataCell
    {
        private readonly int _pointsPerCell;
        private readonly HgtCellCoords _coords;

        protected HgtDataCellBase(int pointsPerCell, HgtCellCoords coords)
        {
            _pointsPerCell = pointsPerCell;
            _coords = coords;
        }

        public double GetElevation(double latitude, double longitude)
        {
            int localLat = (int)((latitude - _coords.Lat) * _pointsPerCell);
            int localLon = (int)((longitude - _coords.Lon) * _pointsPerCell);
            int bytesPos = ((_pointsPerCell - localLat - 1) * _pointsPerCell * 2) + localLon * 2;

            if (bytesPos < 0 || bytesPos > _pointsPerCell * _pointsPerCell * 2)
                throw new ArgumentException("latitude or longitude out of range");

            return elevationAtOffset(bytesPos);
        }

        protected abstract double elevationAtOffset(int bytesPos);
    }
}