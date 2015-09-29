using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtDataCell
    {
        long MemorySize { get; }

        [Pure]
        double GetElevation(double latitude, double longitude);

        [Pure]
        Task<double> GetElevationAsync(double latitude, double longitude);
    }
}
