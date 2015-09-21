using System;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm
{
    internal struct PgmCellCords : IEquatable<PgmCellCords>
    {
        private readonly int _lat;
        private readonly int _lon;

        public PgmCellCords(int lat, int lon)
        {
            _lat = lat;
            _lon = lon;
        }

        public int Lat { get { return _lat; } }
        public int Lon { get { return _lon; } }

        public bool Equals(PgmCellCords other)
        {
            return _lat == other._lat && _lon == other._lon;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            return obj is PgmCellCords && Equals((PgmCellCords)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_lat * 397) ^ _lon;
            }
        }

        public static bool operator ==(PgmCellCords left, PgmCellCords right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PgmCellCords left, PgmCellCords right)
        {
            return !left.Equals(right);
        }
    }
}