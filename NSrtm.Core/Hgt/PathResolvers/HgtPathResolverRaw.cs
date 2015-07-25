namespace NSrtm.Core
{
    internal class HgtPathResolverRaw : HgtPathResolverCaching
    {
        protected override string coordsToFilename(HgtCellCoords coords)
        {
            return coords.ToBaseName() + ".hgt";
        }
    }
}