using JetBrains.Annotations;

namespace NSrtm.Core
{
    public interface IHgtPathResolver
    {
        [NotNull]
        string FindFilePath(string directory, HgtCellCoords coords);
    }
}
