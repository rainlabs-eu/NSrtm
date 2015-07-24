namespace NSrtm.Core
{
    public interface IElevationProvider
    {
        /// <summary>
        /// Gets elevation above MSL
        /// </summary>
        /// <param name="latitude">Latitude in degrees</param>
        /// <param name="longitude">Longitude in degrees</param>
        /// <returns></returns>
        double GetElevation(double latitude, double longitude);
    }
}

