using JetBrains.Annotations;

namespace NSrtm.Core
{
    public interface IHgtDataCellFactory
    {
        [NotNull]
        IHgtDataCell GetCellFor(HgtCellCoords coords);
    }
}
