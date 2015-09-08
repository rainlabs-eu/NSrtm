namespace NSrtm.Core.Pgm.GeoidUndulationGrid
{
    internal interface IPgmGeoidUndulationGrid
    {
        double GetUndulation(double latitude, double longitude);
    }
}
