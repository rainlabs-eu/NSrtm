using System;

namespace NSrtm.Core
{
    static internal class HgtUtils
    {
        private const int srtm1PointsPerCell = 3601;
        private const int srtm3PointsPerCell = 1201;
        private const int srtm3Length = srtm3PointsPerCell * srtm3PointsPerCell * 2;
        private const int srtm1Length = srtm1PointsPerCell * srtm1PointsPerCell * 2;


        internal static int PointsPerCellFromDataLength(int length)
        {
            int pointsPerCell;
            switch (length)
            {
                case srtm3Length: // SRTM-3
                    pointsPerCell = srtm3PointsPerCell;
                    break;
                case srtm1Length: // SRTM-1
                    pointsPerCell = srtm1PointsPerCell;
                    break;
                default:
                    throw new ArgumentException(String.Format("Unsupported data length {0}", length), "length");
            }
            return pointsPerCell;
        }

        internal static bool IsDataLengthValid(long length)
        {
            return length == srtm3Length || length == srtm1Length;
        }

        public static bool IsDataLengthValid(int length)
        {
            return IsDataLengthValid((long)length);
        }
    }
}