using System;
using System.IO;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtDataCellInMemoryFactory : IHgtDataCellFactory
    {
        private readonly string _directory;
        private readonly IHgtDataLoader _loader;

        public HgtDataCellInMemoryFactory([NotNull] string directory, [NotNull] IHgtDataLoader loader)
        {
            if (directory == null) throw new ArgumentNullException("directory");
            if (!Directory.Exists(directory)) throw new DirectoryNotFoundException(string.Format("Directory {0} not found", directory));

            _directory = directory;
            _loader = loader;
        }

        public IHgtDataCell GetCellFor(HgtCellCoords coords)
        {
            var data = _loader.LoadFromFile(_directory, coords);
            return new HgtDataCellInMemory(data, HgtUtils.PointsPerCellFromDataLength(data.Length), coords);
        }

        private class HgtDataCellInMemory : HgtDataCellBase
        {
            private readonly byte[] _hgtData;

            internal HgtDataCellInMemory([NotNull] byte[] hgtData, int pointsPerCell, HgtCellCoords coords) : base(pointsPerCell, coords)
            {
                _hgtData = hgtData;
            }

            public override long MemorySize { get { return _hgtData.Length; } }

            protected override double ElevationAtOffset(int bytesPos)
            {
                // Motorola "big endian" order with the most significant byte first
                Int16 elevation = (short)((_hgtData[bytesPos]) << 8 | _hgtData[bytesPos + 1]);
                if (elevation > short.MinValue)
                    return elevation;
                else return Double.NaN;
            }
        }
    }
}
