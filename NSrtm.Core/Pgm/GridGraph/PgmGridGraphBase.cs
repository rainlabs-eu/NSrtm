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

        protected double fromRawDataToUndulation(int rawData)
        {
            if (rawData < 0 || rawData > PgmParameters.MaxValue)
            {
                throw new ArgumentOutOfRangeException("rawData");
            }
            return rawData * PgmParameters.Scale + PgmParameters.Offset;
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
