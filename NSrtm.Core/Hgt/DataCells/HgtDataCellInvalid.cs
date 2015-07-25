using System;

namespace NSrtm.Core
{
    internal class HgtDataCellInvalid : IHgtDataCell
    {
        private static readonly HgtDataCellInvalid invalid = new HgtDataCellInvalid();

        private HgtDataCellInvalid()
        {
        }

        public static HgtDataCellInvalid Invalid { get { return invalid; } }

        public double GetElevation(double latitude, double longitude)
        {
            return Double.NaN;
        }
    }
}