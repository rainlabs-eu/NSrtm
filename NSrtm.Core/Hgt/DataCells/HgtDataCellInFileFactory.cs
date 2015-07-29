using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtDataCellInFileFactory : IHgtDataCellFactory
    {
        private readonly IHgtPathResolver _pathResolver;

        public HgtDataCellInFileFactory([NotNull] IHgtPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",Justification = "Returned cell is disposable")]
        public IHgtDataCell GetCellFor(HgtCellCoords coords)
        {
            var path = _pathResolver.FindFilePath(coords);

            FileStream file = null;
            try
            {
                int fileSize = (int)new FileInfo(path).Length;
                file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return new HgtDataCellInFile(file, HgtUtils.PointsPerCellFromDataLength(fileSize), coords);
            }
            catch (Exception)
            {
                if (file != null) file.Dispose();
                throw;
            }
        }

        public Task<IHgtDataCell> GetCellForAsync(HgtCellCoords coords)
        {
            return Task.FromResult(GetCellFor(coords));
        }

        public sealed class HgtDataCellInFile : HgtDataCellBase, IDisposable
        {
            private readonly FileStream _file;
            private readonly object _lock = new object();

            internal HgtDataCellInFile([NotNull] FileStream file, int fileSize, HgtCellCoords coords) : base(fileSize, coords)
            {
                _file = file;
            }

            public override long MemorySize { get { return 0; } }

            public void Dispose()
            {
                _file.Dispose();
            }

            public override Task<double> GetElevationAsync(double latitude, double longitude)
            {
                return Task.FromResult(GetElevation(latitude, longitude));
            }

            protected override double ElevationAtOffset(int bytesPos)
            {
                lock (_lock)
                {
                    _file.Seek(bytesPos, SeekOrigin.Begin);
                    Int16 elevation = (Int16)(_file.ReadByte() << 8 | _file.ReadByte());
                    if (elevation > Int16.MinValue)
                        return elevation;
                    else return Double.NaN;
                }
            }
        }
    }
}
