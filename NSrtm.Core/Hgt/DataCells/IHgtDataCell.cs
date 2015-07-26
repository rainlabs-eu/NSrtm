namespace NSrtm.Core
{
    internal interface IHgtDataCell
    {
        double GetElevation(double latitude, double longitude);

        long MemorySize { get; }
    }
}