namespace NSrtm.Core.Pgm.GeoidUndulationGrid
{
    public interface IPgmGeoidUndulationGrid
    {
        double GetUndulation(double latitude, double longitude);
    }
}
