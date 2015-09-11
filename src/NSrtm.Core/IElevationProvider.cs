using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    /// <summary>
    ///     Provides elevation for WGS84 locations.
    /// </summary>
    public interface IElevationProvider
    {
        /// <summary>
        ///     Gets elevation above MSL
        /// </summary>
        /// <param name="latitude">Latitude in degrees in WGS84 datum</param>
        /// <param name="longitude">Longitude in degrees in WGS84 datum</param>
        /// <returns></returns>
        double GetElevation(double latitude, double longitude);

        /// <summary>
        ///     Gets elevation above MSL
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        Task<double> GetElevationAsync(double latitude, double longitude);

        /// <summary>
        /// Base level (for this level elevation provider returns zero)
        /// </summary>
        Level ElevationBase { get; }

        /// <summary>
        /// Target Level - this level measured relatively to Base is returned
        /// </summary>
        Level ElevationTarget { get; }
    }
}
