namespace NSrtm.Core.Pgm.Grid
{
    public interface IGrid
    {
        double GetClosestUndulationValue(double latitude, double longitude);
        GridConstants Parameters { get; }
    }
}
