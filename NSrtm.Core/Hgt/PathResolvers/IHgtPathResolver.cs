using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtPathResolver
    {
        [NotNull]
        string FindFilePath([NotNull] string directory, HgtCellCoords coords);
    }
}
