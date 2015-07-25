namespace NSrtm.Core
{
    internal class HgtPathResolverZip : HgtPathResolverCaching
    {
        protected override string coordsToFilename(HgtCellCoords coords)
        {
            return coords.ToBaseName() + ".hgt.zip";
            
        }
    }
}