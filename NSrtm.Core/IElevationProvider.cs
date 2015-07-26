using JetBrains.Annotations;

namespace NSrtm.Core
{
    /// <summary>
    ///     Provides elevation of ground for WGS84 locations.
    /// </summary>
    public interface IElevationProvider
    {
        /// <summary>
        ///     Short name describing implementation. Used for UI/Demos where different implementations are available.
        /// </summary>
        [NotNull] string Name { get; }

        /// <summary>
        ///     More descriptive info about implementation. Used for UI/Demos where different implementations are available.
        /// </summary>
        [NotNull] string Description { get; }

        /// <summary>
        ///     Gets elevation above MSL
        /// </summary>
        /// <param name="latitude">Latitude in degrees in WGS84 datum</param>
        /// <param name="longitude">Longitude in degrees in WGS84 datum</param>
        /// <returns></returns>
        double GetElevation(double latitude, double longitude);
    }
}
