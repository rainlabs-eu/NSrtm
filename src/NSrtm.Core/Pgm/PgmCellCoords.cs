using System;
using JetBrains.Annotations;
using NSrtm.Core.Pgm.DataDesciption;

namespace NSrtm.Core.Pgm
{
    internal struct PgmCellCoords : IEquatable<PgmCellCoords>
    {
        private readonly double _lat;
        private readonly double _lon;

        public PgmCellCoords(double lat, double lon)
        {
            _lat = lat;
            _lon = lon;
        }

        public double Lat { get { return _lat; } }
        public double Lon { get { return _lon; } }

        public bool Equals(PgmCellCoords other)
        {
            return _lat.Equals(other._lat) && _lon.Equals(other._lon);
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PgmCellCoords && Equals((PgmCellCoords)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_lat.GetHashCode() * 397) ^ _lon.GetHashCode();
            }
        }

        public static bool operator ==(PgmCellCoords left, PgmCellCoords right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PgmCellCoords left, PgmCellCoords right)
        {
            return !left.Equals(right);
        }

        public static PgmCellCoords ForCoordinatesUsingDescription(double latitude, double longitude, PgmDataDescription dataDescription)
        {
            var verticalPositonSteps = dataDescription.OriginLat - latitude / dataDescription.LatitudeIncrementDegrees;
            var nodeVerticalPosition = Math.Floor(verticalPositonSteps) * dataDescription.LatitudeIncrementDegrees;
            var horizontalPositionSteps = dataDescription.OriginLon - longitude / dataDescription.LongitudeIncrementDegrees;
            var nodeHorizontalPosition = Math.Floor(horizontalPositionSteps) * dataDescription.LongitudeIncrementDegrees;
            return new PgmCellCoords(nodeVerticalPosition, nodeHorizontalPosition);
        }
    }
}
