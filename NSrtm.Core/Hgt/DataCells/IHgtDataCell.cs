namespace NSrtm.Core
{
    public interface IHgtDataCell
    {
        double GetElevation(double latitude, double longitude);

        long MemorySize { get; }
    }
}