using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal sealed class HgtDataLoaderFromRaw : HgtDataLoaderFromFileStreamBase
    {
        public HgtDataLoaderFromRaw([NotNull] IHgtPathResolver pathResolver)
            : base(pathResolver)
        {
        }

        protected override byte[] LoadHgtDataFromFile(HgtCellCoords coords, string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadHgtDataFromStream(fileStream);
            }
        }

        protected override async Task<byte[]> LoadHgtDataFromFileAsync(HgtCellCoords coords, string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await LoadHgtDataFromStreamAsync(fileStream);
            }
        }
    }
}
