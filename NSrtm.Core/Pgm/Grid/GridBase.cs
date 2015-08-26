using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSrtm.Core.Pgm.Grid
{
    public abstract class GridBase : IGrid
    {
        protected readonly GridConstants PgmParameters;

        protected GridBase(GridConstants pgmParameters)
        {
            PgmParameters = pgmParameters;
        }

        public double GetClosestData(double latitude, double longitude)
        {
            var position = getClosestDataPosition(latitude, longitude);
            return getDataFrom(position);
        }

        private int getClosestDataPosition(double latitude, double longitude)
        {
            var lonStep = PgmParameters.GridWidthPoints / 360.0;
            var latStep = (PgmParameters.GridHeightPoints - 1) / 180.0;

            int latPoints = (int)Math.Round((PgmParameters.OrginLat - latitude) * latStep);
            int lonPoints = (int)Math.Round((longitude - PgmParameters.OrginLon) * lonStep);
            int dataPosition = (lonPoints + latPoints * PgmParameters.GridWidthPoints);

            if (dataPosition < 0 || dataPosition > PgmParameters.NumberOfPoints)
                throw new ArgumentOutOfRangeException("dataPosition");
            return dataPosition;
        }

        protected double fromRawData(int data)
        {
            return data * PgmParameters.Scale + PgmParameters.Offset;
        }

        protected abstract double getDataFrom(int position);

        public GridConstants Parameters { get { return PgmParameters; } }
    }
}
