using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal interface IHgtDataCellFactory
    {
        [NotNull]
        IHgtDataCell GetCellFor(HgtCellCoords coords);

        [NotNull]
        Task<IHgtDataCell> GetCellForAsync(HgtCellCoords coords);
    }
}
