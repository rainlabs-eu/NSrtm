using System;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal struct HgtCellCoords : IEquatable<HgtCellCoords>
    {
        private readonly int _lat;
        private readonly int _lon;

        private HgtCellCoords(int lat, int lon)
        {
            _lat = lat;
            _lon = lon;
        }

        public int Lat { get { return _lat; } }
        public int Lon { get { return _lon; } }

        public bool Equals(HgtCellCoords other)
        {
            return _lat == other._lat && _lon == other._lon;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            return obj is HgtCellCoords && Equals((HgtCellCoords)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_lat * 397) ^ _lon;
            }
        }

        public static bool operator ==(HgtCellCoords left, HgtCellCoords right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HgtCellCoords left, HgtCellCoords right)
        {
            return !left.Equals(right);
        }

        public static HgtCellCoords ForLatLon(double latitude, double longitude)
        {
            return new HgtCellCoords((int)Math.Floor(latitude), (int)Math.Floor(longitude));
        }

        public string ToBaseName()
        {
            return String.Format("{0}{1:D2}{2}{3:D3}",
                                 _lat < 0 ? "S" : "N",
                                 Math.Abs(_lat),
                                 _lon < 0 ? "W" : "E",
                                 Math.Abs(_lon));
        }
    }
}
