using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal sealed class HgtDataLoaderFromZip : HgtDataLoaderFromFileStreamBase
    {
        public HgtDataLoaderFromZip([NotNull] IHgtPathResolver pathResolver) : base(pathResolver)
        {
        }

        protected override byte[] LoadHgtDataFromFile(HgtCellCoords coords, string filePath)
        {
            using (var zipArchive = ZipFile.OpenRead(filePath))
            {
                var entry = zipArchive.Entries.Single();

                long length = entry.Length;
                if (!HgtUtils.IsDataLengthValid(length))
                    throw new HgtFileInvalidException(coords, string.Format("Invalid length - {0} bytes", length));

                using (var zipStream = entry.Open())
                {
                    return LoadHgtDataFromStream(zipStream);
                }
            }
        }

        protected override async Task<byte[]> LoadHgtDataFromFileAsync(HgtCellCoords coords, string filePath)
        {
            using (var zipArchive = ZipFile.OpenRead(filePath))
            {
                var entry = zipArchive.Entries.Single();

                long length = entry.Length;
                if (!HgtUtils.IsDataLengthValid(length))
                    throw new HgtFileInvalidException(coords, string.Format("Invalid length - {0} bytes", length));

                using (var zipStream = entry.Open())
                {
                    return await LoadHgtDataFromStreamAsync(zipStream);
                }
            }
        }
    }
}
