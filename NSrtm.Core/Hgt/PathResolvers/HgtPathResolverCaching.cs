using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal abstract class HgtPathResolverCaching : IHgtPathResolver
    {
        private readonly ConcurrentDictionary<HgtCellCoords, string> _cache = new ConcurrentDictionary<HgtCellCoords, string>();

        public string FindFilePath(string directory, HgtCellCoords coords)
        {
            return _cache.GetOrAdd(coords, c => findPathForFile(directory, c));
        }

        [NotNull]
        private string findPathForFile([NotNull] string directory, HgtCellCoords coords)
        {
            string filename = coordsToFilename(coords);
            string[] potentialPaths =
            {
                Path.Combine(directory, filename),
                Path.Combine(directory, "Eurasia", filename),
                Path.Combine(directory, "Africa", filename),
                Path.Combine(directory, "South_America", filename),
                Path.Combine(directory, "North_America", filename),
                Path.Combine(directory, "Islands", filename),
            };

            var path = potentialPaths.FirstOrDefault(File.Exists);
            if (path != null) return path;

            var foundfile = new DirectoryInfo(directory).EnumerateFiles(filename, SearchOption.AllDirectories)
                                                        .FirstOrDefault();
            if (foundfile != null) return foundfile.FullName;
            else throw new HgtFileNotFoundException(coords);
        }

        [NotNull]
        protected abstract string coordsToFilename(HgtCellCoords coords);
    }
}
