namespace NSrtm.Core.Pgm.GridGraph
{
    public interface IPgmGridGraph
    {
        double GetUndulation(double latitude, double longitude);
        PgmDataDescription Parameters { get; }
    }
}
