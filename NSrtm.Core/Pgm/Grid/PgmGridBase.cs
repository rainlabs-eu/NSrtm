using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSrtm.Core.Pgm.Grid
{
    public abstract class PgmGridBase : IPgmGrid
    {
        protected readonly PgmGridConstants PgmParameters;

        protected PgmGridBase(PgmGridConstants pgmParameters)
        {
            PgmParameters = pgmParameters;
        }

        public double GetClosestUndulationValue(double latitude, double longitude)
        {
            var position = getClosestGridPosition(latitude, longitude);
            return getUndulationFrom(position);
        }

        private int getClosestGridPosition(double latitude, double longitude)
        {
            var lonStep = PgmParameters.GridWidthPoints / 360.0;
            var latStep = (PgmParameters.GridHeightPoints - 1) / 180.0;

            int latPoints = (int)Math.Round((PgmParameters.OrginLat - latitude) * latStep);
            int lonPoints = (int)Math.Round((longitude - PgmParameters.OrginLon) * lonStep);
            int position = (lonPoints + latPoints * PgmParameters.GridWidthPoints);

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

        public PgmGridConstants Parameters { get { return PgmParameters; } }
    }
}
