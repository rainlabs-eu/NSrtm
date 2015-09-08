using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtPathResolver
    {
        [NotNull]
        string FindFilePath(HgtCellCoords coords);
    }
}
