using System.IO;
using System.IO.Compression;
using System.Linq;

namespace NSrtm.Core
{
    class HgtDataLoaderFromZip : IHgtDataLoader
    {

        private readonly IHgtPathResolver _pathResolver;

        public HgtDataLoaderFromZip(IHgtPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        public byte[] LoadFromFile(string directory, HgtCellCoords coords)
        {
            var filePath = _pathResolver.FindFilePath(directory, coords);

            byte[] hgtData;
            using (var zipArchive = ZipFile.OpenRead(filePath))
            {
                var entry = zipArchive.Entries.Single();

                long length = entry.Length;
                if (length != HgtUtils.Srtm3Length && length != HgtUtils.Srtm1Length)
                    throw new HgtFileInvalidException(coords, string.Format("Invalid length - {0} bytes", length));

                using (var zipStream = entry.Open())
                {
                    using (var memory = new MemoryStream())
                    {
                        zipStream.CopyTo(memory);
                        hgtData = memory.ToArray();
                    }
                }
            }
            return hgtData;
        }
    }
}