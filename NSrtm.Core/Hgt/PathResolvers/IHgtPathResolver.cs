using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtPathResolver
    {
        [NotNull]
        string FindFilePath(string directory, HgtCellCoords coords);
    }
}
