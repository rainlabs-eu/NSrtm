using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace NSrtm.Core
{

    internal class HgtDataCellInMemory : HgtDataCellBase
    {
        private readonly byte[] _hgtData;

        internal HgtDataCellInMemory(byte[] hgtData, int pointsPerCell, HgtCellCoords coords) : base(pointsPerCell, coords)
        {
            _hgtData = hgtData;
        }

        protected override double elevationAtOffset(int bytesPos)
        {
            // Motorola "big endian" order with the most significant byte first
            Int16 elevation = (short)((_hgtData[bytesPos]) << 8 | _hgtData[bytesPos + 1]);
            if (elevation > short.MinValue)
                return elevation;
            else return Double.NaN;
        }
    }
}
