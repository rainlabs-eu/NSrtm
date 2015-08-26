namespace NSrtm.Core.Pgm.Grid
{
    public interface IGrid
    {
        double GetClosestData(double latitude, double longitude);
        GridConstants Parameters { get; }
    }
}
