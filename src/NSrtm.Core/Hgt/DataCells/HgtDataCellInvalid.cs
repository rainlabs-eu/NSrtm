using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtDataCellInvalid : IHgtDataCell
    {
        private HgtDataCellInvalid()
        {
        }

        public double GetElevation(double latitude, double longitude)
        {
            return Double.NaN;
        }

        public long MemorySize { get { return 0; } }

        [NotNull]
        public Task<double> GetElevationAsync(double latitude, double longitude)
        {
            return invalidElevationTask;
        }

        #region Static Members

        private static readonly HgtDataCellInvalid invalid = new HgtDataCellInvalid();
        private static readonly Task<double> invalidElevationTask = Task.FromResult(Double.NaN);

        [NotNull]
        public static HgtDataCellInvalid Invalid { get { return invalid; } }

        #endregion
    }
}
