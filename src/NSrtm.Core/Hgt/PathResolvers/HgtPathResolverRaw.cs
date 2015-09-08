using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtPathResolverRaw : HgtPathResolverCaching
    {
        public HgtPathResolverRaw([NotNull] string directory) : base(directory)
        {
        }

        protected override string coordsToFilename(HgtCellCoords coords)
        {
            return coords.ToBaseName() + ".hgt";
        }
    }
}