namespace NSrtm.Core.Pgm.GridGraph
{
    public interface IPgmGridGraph
    {
        double GetClosestUndulationValue(double latitude, double longitude);
        PgmGridGraphConstants Parameters { get; }
    }
}
