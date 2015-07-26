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
    }
}