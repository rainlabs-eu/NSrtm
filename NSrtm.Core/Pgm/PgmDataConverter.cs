using System;

namespace NSrtm.Core.Pgm
{
    internal static class PgmDataConverter
    {
        public static double RawToEgmFormat(this UInt16 rawData, PgmDataDescription pgmParameters)
        {
            if (rawData > pgmParameters.MaxValue)
            {
                throw new ArgumentOutOfRangeException("rawData");
            }
            return rawData * pgmParameters.Scale + pgmParameters.Offset;
        }

        public static int CoordinatesToClosestGridPosition(double latitude, double longitude, PgmDataDescription dataDescription)
        {
            var gridAbsoluteLat = (dataDescription.OriginLat - latitude) * dataDescription.LatitudeIncrement;
            int closestAccessibleGridLat = (int)Math.Round(gridAbsoluteLat);
            var gridAbsoluteLon = (longitude - dataDescription.OriginLon) * dataDescription.LongitudeIncrement;
            int closestAccessibleGridLon = (int)Math.Round(gridAbsoluteLon);
            int closestAccessiblePosition = (closestAccessibleGridLon + closestAccessibleGridLat * dataDescription.GridGraphWidthPoints);

            if (closestAccessiblePosition < 0 || closestAccessiblePosition > dataDescription.NumberOfPoints)
                throw new InvalidOperationException(String.Format("Position is out of range. Probably latitude {0} or longitude {1} are out of range.",
                                                                  latitude,
                                                                  longitude));
            return closestAccessiblePosition;
        }
    }
}
