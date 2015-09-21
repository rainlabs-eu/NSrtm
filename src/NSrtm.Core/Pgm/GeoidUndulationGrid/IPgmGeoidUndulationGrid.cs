using NSrtm.Core.Pgm.DataDesciption;

namespace NSrtm.Core.Pgm.GeoidUndulationGrid
{
    public interface IPgmGeoidUndulationGrid
    {
        double GetUndulation(double latitude, double longitude);
        PgmDataDescription PgmParameters { get; }
    }
}
