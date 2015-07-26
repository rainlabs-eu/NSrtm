using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtDataLoader
    {
        [NotNull]
        byte[] LoadFromFile(string directory, HgtCellCoords coords);
    }
}