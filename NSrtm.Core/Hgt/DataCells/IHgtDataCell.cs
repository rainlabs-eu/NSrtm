using System.Threading.Tasks;

namespace NSrtm.Core
{
    internal interface IHgtDataCell
    {
        double GetElevation(double latitude, double longitude);

        long MemorySize { get; }
        Task<double> GetElevationAsync(double latitude, double longitude);
    }
}