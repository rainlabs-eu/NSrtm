using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtDataCellInMemoryFactory : IHgtDataCellFactory
    {
        private readonly IHgtDataLoader _loader;

        public HgtDataCellInMemoryFactory([NotNull] IHgtDataLoader loader)
        {
            _loader = loader;
        }

        public IHgtDataCell GetCellFor(HgtCellCoords coords)
        {
            var data = _loader.LoadFromFile(coords);
            return new HgtDataCellInMemory(data, HgtUtils.PointsPerCellFromDataLength(data.Length), coords);
        }

        public async Task<IHgtDataCell> GetCellForAsync(HgtCellCoords coords)
        {
            var data = await _loader.LoadFromFileAsync(coords);

            return new HgtDataCellInMemory(data, HgtUtils.PointsPerCellFromDataLength(data.Length), coords);
        }

        private class HgtDataCellInMemory : HgtDataCellBase
        {
            private readonly byte[] _hgtData;

            internal HgtDataCellInMemory([NotNull] byte[] hgtData, int pointsPerCell, HgtCellCoords coords)
                : base(pointsPerCell, coords)
            {
                _hgtData = hgtData;
            }

            public override long MemorySize { get { return _hgtData.Length; } }

            [NotNull]
            public override Task<double> GetElevationAsync(double latitude, double longitude)
            {
                return Task.FromResult(GetElevation(latitude, longitude));
            }

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
