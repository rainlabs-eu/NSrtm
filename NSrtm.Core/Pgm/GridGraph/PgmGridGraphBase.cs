using System;

namespace NSrtm.Core.Pgm.GridGraph
{
    public abstract class PgmGridGraphBase : IPgmGridGraph
    {
        private readonly PgmDataDescription _pgmParameters;

        protected PgmGridGraphBase(PgmDataDescription pgmParameters)
        {
            _pgmParameters = pgmParameters;
        }

        private int getClosestPosition(double latitude, double longitude)
        {
            int latPoints = (int)Math.Round((_pgmParameters.OriginLat - latitude) * _pgmParameters.LatitudeIncrement);
            int lonPoints = (int)Math.Round((longitude - _pgmParameters.OriginLon) * _pgmParameters.LongitudeIncrement);
            int position = (lonPoints + latPoints * _pgmParameters.GridGraphWidthPoints);

            if (position < 0 || position > _pgmParameters.NumberOfPoints)
                throw new ArgumentOutOfRangeException("position");
            return position;
        }

        protected abstract double GetUndulationFrom(int position);

        public double GetUndulation(double latitude, double longitude)
        {
            var position = getClosestPosition(latitude, longitude);
            return GetUndulationFrom(position);
        }

        public PgmDataDescription Parameters { get { return _pgmParameters; } }
    }
}
