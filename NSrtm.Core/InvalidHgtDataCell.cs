using System;

namespace NSrtm.Core
{
    internal class InvalidHgtDataCell : IHgtDataCell
    {
        private static readonly InvalidHgtDataCell invalid = new InvalidHgtDataCell();

        private InvalidHgtDataCell()
        {
        }

        public static InvalidHgtDataCell Invalid { get { return invalid; } }

        public double GetElevation(double latitude, double longitude)
        {
            return Double.NaN;
        }
    }
}