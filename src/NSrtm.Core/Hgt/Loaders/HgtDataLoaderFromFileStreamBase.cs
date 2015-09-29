using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal abstract class HgtDataLoaderFromFileStreamBase : IHgtDataLoader
    {
        private readonly IHgtPathResolver _pathResolver;

        protected HgtDataLoaderFromFileStreamBase([NotNull] IHgtPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        protected abstract byte[] LoadHgtDataFromFile(HgtCellCoords coords, [NotNull] string filePath);
        protected abstract Task<byte[]> LoadHgtDataFromFileAsync(HgtCellCoords coords, [NotNull] string filePath);

        protected static byte[] LoadHgtDataFromStream(Stream zipStream)
        {
            byte[] hgtData;
            using (var memory = new MemoryStream())
            {
                zipStream.CopyTo(memory);
                hgtData = memory.ToArray();
            }
            return hgtData;
        }

        protected static async Task<byte[]> LoadHgtDataFromStreamAsync(Stream zipStream)
        {
            byte[] hgtData;
            using (var memory = new MemoryStream())
            {
                await zipStream.CopyToAsync(memory);
                hgtData = memory.ToArray();
            }
            return hgtData;
        }

        [NotNull]
        public byte[] LoadFromFile(HgtCellCoords coords)
        {
            var filePath = _pathResolver.FindFilePath(coords);

            return LoadHgtDataFromFile(coords, filePath);
        }

        public Task<byte[]> LoadFromFileAsync(HgtCellCoords coords)
        {
            var filePath = _pathResolver.FindFilePath(coords);

            return LoadHgtDataFromFileAsync(coords, filePath);
        }
    }
}
