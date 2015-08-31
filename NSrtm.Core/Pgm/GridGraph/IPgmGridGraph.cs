namespace NSrtm.Core.Pgm.Grid
{
    public interface IPgmGrid
    {
        double GetClosestUndulationValue(double latitude, double longitude);
        PgmGridConstants Parameters { get; }
    }
}
