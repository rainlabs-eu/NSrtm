using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtPathResolverZip : HgtPathResolverCaching
    {
        public HgtPathResolverZip([NotNull] string directory) : base(directory)
        {
        }

        protected override string coordsToFilename(HgtCellCoords coords)
        {
            return coords.ToBaseName() + ".hgt.zip";
        }
    }
}