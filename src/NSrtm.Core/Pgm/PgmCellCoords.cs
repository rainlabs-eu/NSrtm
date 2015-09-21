using System;
using JetBrains.Annotations;
using NSrtm.Core.Pgm.DataDesciption;

namespace NSrtm.Core.Pgm
{
    internal struct PgmCellCords : IEquatable<PgmCellCords>
    {
        private readonly double _lat;
        private readonly double _lon;

        public PgmCellCords(double lat, double lon)
        {
            _lat = lat;
            _lon = lon;
        }

        public double Lat { get { return _lat; } }
        public double Lon { get { return _lon; } }

        public bool Equals(PgmCellCords other)
        {
            return _lat.Equals(other._lat) && _lon.Equals(other._lon);
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PgmCellCords && Equals((PgmCellCords)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_lat.GetHashCode() * 397) ^ _lon.GetHashCode();
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

    internal static class PgmCellCoordsExtensions
    {
        public static PgmCellCords FromLatLon(double latitude, double longitude, PgmDataDescription dataDescription)
        {
            var mainNodeLat = Math.Floor(dataDescription.OriginLat - latitude / dataDescription.LatitudeIncrementDegrees) *
                              dataDescription.LatitudeIncrementDegrees;
            var mainNodeLot = Math.Floor(dataDescription.OriginLon - longitude / dataDescription.LongitudeIncrementDegrees) *
                              dataDescription.LongitudeIncrementDegrees;
            return new PgmCellCords(mainNodeLat, mainNodeLot);
        }
    }
}
