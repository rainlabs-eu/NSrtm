using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtDataLoader
    {
        [NotNull]
        byte[] LoadFromFile(HgtCellCoords coords);
    }
}