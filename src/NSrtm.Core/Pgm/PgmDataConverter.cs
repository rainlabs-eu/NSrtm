using System;
using NSrtm.Core.Pgm.DataDesciption;

namespace NSrtm.Core.Pgm
{
    internal static class PgmDataConverter
    {
        public static double RawToFinalFormat(this UInt16 rawData, PgmDataDescription pgmParameters)
        {
            if (rawData > pgmParameters.MaxValue)
            {
                throw new ArgumentOutOfRangeException("rawData");
            }
            return rawData * pgmParameters.Scale + pgmParameters.Offset;
        }

        public static int CoordinatesToClosestGridPosition(double latitude, double longitude, PgmDataDescription dataDescription)
        {
            validatePgmDataDescription(dataDescription);

            var longitude0To360 = (longitude + 360) % 360;

            var gridAbsoluteLat = (dataDescription.OriginLat - latitude) / dataDescription.LatitudeIncrementDegrees;
            int closestAccessibleGridLat = (int)Math.Round(gridAbsoluteLat);
            var gridAbsoluteLon = (longitude0To360 - dataDescription.OriginLon) / dataDescription.LongitudeIncrementDegrees;
            int closestAccessibleGridLon = (int)Math.Round(gridAbsoluteLon);
            int closestAccessiblePosition = (closestAccessibleGridLon + closestAccessibleGridLat * dataDescription.GridGraphWidthPoints);

            if (closestAccessiblePosition < 0 || closestAccessiblePosition > dataDescription.NumberOfPoints)
                throw new InvalidOperationException(String.Format("Position is out of range. Probably latitude {0} or longitude {1} are out of range.",
                                                                  latitude,
                                                                  longitude));
            return closestAccessiblePosition;
        }

        private static void validatePgmDataDescription(PgmDataDescription dataDescription)
        {
            if (dataDescription.OriginLat != 90 || dataDescription.OriginLon != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dataDescription),
                    String.Format("Coordinates of the origin are {0} {1}, but should be 90 0.",
                        dataDescription.OriginLat,
                        dataDescription.OriginLon));
            }

            if (dataDescription.MaxValue <= byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(dataDescription),
                    String.Format($"Max value of {dataDescription.MaxValue} indicates that data uses one byte per pixel. This mode is not supported"));
            }
        }
    }
}
