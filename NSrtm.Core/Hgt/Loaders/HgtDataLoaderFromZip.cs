using System.IO;
using System.IO.Compression;
using System.Linq;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtDataLoaderFromZip : IHgtDataLoader
    {

        private readonly IHgtPathResolver _pathResolver;

        public HgtDataLoaderFromZip([NotNull] IHgtPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        [NotNull]
        public byte[] LoadFromFile(HgtCellCoords coords)
        {
            var filePath = _pathResolver.FindFilePath(coords);

            byte[] hgtData;
            using (var zipArchive = ZipFile.OpenRead(filePath))
            {
                var entry = zipArchive.Entries.Single();

                long length = entry.Length;
                if (!HgtUtils.IsDataLengthValid(length))
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