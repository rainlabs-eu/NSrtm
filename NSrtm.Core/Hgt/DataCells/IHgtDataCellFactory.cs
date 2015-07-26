using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtDataCellFactory
    {
        [NotNull]
        IHgtDataCell GetCellFor(HgtCellCoords coords);
    }
}
