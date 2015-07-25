using System;
using System.Globalization;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    static internal class HgtUtils
    {
        public const int Srtm3Length = 1201 * 1201 * 2;
        public const int Srtm1Length = 3601 * 3601 * 2;


        internal static int PointsPerCellFromDataLength(int length)
        {
            int pointsPerCell;
            switch (length)
            {
                case Srtm3Length: // SRTM-3
                    pointsPerCell = 1201;
                    break;
                case Srtm1Length: // SRTM-1
                    pointsPerCell = 3601;
                    break;
                default:
                    throw new ArgumentException(String.Format("Unsupported data length {0}", length), "length");
            }
            return pointsPerCell;
        }
    }
}