using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace NSrtm.Core
{
    public class HgtDataCellInMemoryMappedFactory : IHgtDataCellFactory
    {
        private readonly string _directory;
        private readonly IHgtPathResolver _pathResolver;

        public HgtDataCellInMemoryMappedFactory(string directory, IHgtPathResolver pathResolver)
        {
            _directory = directory;
            _pathResolver = pathResolver;
        }

        public IHgtDataCell GetCellFor(HgtCellCoords coords)
        {
            var path = _pathResolver.FindFilePath(_directory, coords);

            MemoryMappedFile file = null;
            MemoryMappedViewAccessor accessor = null;
            try
            {
                int fileSize = (int)new FileInfo(path).Length;
                file = MemoryMappedFile.CreateFromFile(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read),
                                                       path,
                                                       fileSize,
                                                       MemoryMappedFileAccess.Read,
                                                       new MemoryMappedFileSecurity(),
                                                       HandleInheritability.None,
                                                       false);
                accessor = file.CreateViewAccessor();
                return new HgtDataMemoryMappedCell(file, accessor, HgtUtils.PointsPerCellFromDataLength(fileSize), coords);
            }
            catch(Exception)
            {
                if (file != null) file.Dispose();
                if (accessor != null) accessor.Dispose();
                throw;
            }
        }
    }

    public sealed class HgtDataMemoryMappedCell : HgtDataCellBase, IDisposable
    {
        private readonly MemoryMappedFile _file;
        private readonly MemoryMappedViewAccessor _accessor;

        internal HgtDataMemoryMappedCell(MemoryMappedFile file, MemoryMappedViewAccessor accessor, int fileSize, HgtCellCoords coords) : base(fileSize, coords)
        {
            _file = file;
            _accessor = accessor;
        }

        public void Dispose()
        {
            _file.Dispose();
            _accessor.Dispose();
        }

        protected override double ElevationAtOffset(int bytesPos)
        {
            Int16 elevation = (Int16)(_accessor.ReadByte(bytesPos) << 8 | _accessor.ReadByte(bytesPos + 1));
            if (elevation > Int16.MinValue)
                return elevation;
            else return Double.NaN;
        }
    }
}
