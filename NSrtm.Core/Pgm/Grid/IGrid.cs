namespace NSrtm.Core.Pgm.Grid
{
    public interface IGridData
    {
        double GetClosestData(double latitude, double longitude);
        GridConstants parameters { get; }
    }
}
