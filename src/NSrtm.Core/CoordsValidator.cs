using System;

namespace NSrtm.Core
{
    internal static class CoordsValidator
    {
        private static readonly Tuple<double, double> _latitudeRange = Tuple.Create(-90.0, 90.0);
        private static readonly Tuple<double, double> _longitudeRange = Tuple.Create(-180.0, 180.0);

        public static void ThrowIfNotValidWgs84(double latitude, double longitude)
        {
            throwIfOutOfRange(latitude, _latitudeRange, nameof(latitude));
            throwIfOutOfRange(longitude, _longitudeRange, nameof(longitude));
        }

        private static void throwIfOutOfRange(double value, Tuple<double, double> range, string parameterName)
        {
            if(value < range.Item1 || value > range.Item2)
                throw new ArgumentOutOfRangeException(parameterName, 
                    $"Given {parameterName} = {value} is out of range {range.Item1} to {range.Item2}");
        }
    }
}