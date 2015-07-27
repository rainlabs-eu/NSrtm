using System.IO;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtDataLoaderFromRaw : IHgtDataLoader
    {
        private readonly IHgtPathResolver _pathResolver;

        public HgtDataLoaderFromRaw([NotNull] IHgtPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        public byte[] LoadFromFile(HgtCellCoords coords)
        {
            var filePath = _pathResolver.FindFilePath(coords);

            var hgtData = File.ReadAllBytes(filePath);
            int length = hgtData.Length;
            if (!HgtUtils.IsDataLengthValid(length))
                throw new HgtFileInvalidException(coords, string.Format("Invalid length - {0} bytes", length));

            return hgtData;
        }
    }
}
