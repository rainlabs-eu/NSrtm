using System;

namespace NSrtm.Core.Pgm.GridGraph
{
    public abstract class PgmGridGraphBase : IPgmGridGraph
    {
        protected readonly PgmDataDescription PgmParameters;

        protected PgmGridGraphBase(PgmDataDescription pgmParameters)
        {
            PgmParameters = pgmParameters;
        }

        private int getClosestPosition(double latitude, double longitude)
        {
            int latPoints = (int)Math.Round((PgmParameters.OriginLat - latitude) * PgmParameters.LatitudeIncrement);
            int lonPoints = (int)Math.Round((longitude - PgmParameters.OriginLon) * PgmParameters.LongitudeIncrement);
            int position = (lonPoints + latPoints * PgmParameters.GridGraphWidthPoints);

            if (position < 0 || position > PgmParameters.NumberOfPoints)
                throw new ArgumentOutOfRangeException("position");
            return position;
        }

        protected abstract double getUndulationFrom(int position);

        public double GetUndulation(double latitude, double longitude)
        {
            var position = getClosestPosition(latitude, longitude);
            return getUndulationFrom(position);
        }

        public PgmDataDescription Parameters { get { return PgmParameters; } }
    }
}
