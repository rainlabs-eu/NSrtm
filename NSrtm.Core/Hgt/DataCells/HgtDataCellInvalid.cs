using System;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtDataCellInvalid : IHgtDataCell
    {
        private static readonly HgtDataCellInvalid invalid = new HgtDataCellInvalid();

        private HgtDataCellInvalid()
        {
        }

        [NotNull] public static HgtDataCellInvalid Invalid { get { return invalid; } }

        public double GetElevation(double latitude, double longitude)
        {
            return Double.NaN;
        }

        public long MemorySize { get { return 0; } }
    }
}