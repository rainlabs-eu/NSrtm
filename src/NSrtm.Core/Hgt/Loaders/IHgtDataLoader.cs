using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtDataLoader
    {
        [NotNull]
        byte[] LoadFromFile(HgtCellCoords coords);

        Task<byte[]> LoadFromFileAsync(HgtCellCoords coords);
    }
}
