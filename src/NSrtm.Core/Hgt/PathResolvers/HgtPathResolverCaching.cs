using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal abstract class HgtPathResolverCaching : IHgtPathResolver
    {
        private readonly ConcurrentDictionary<HgtCellCoords, string> _cache = new ConcurrentDictionary<HgtCellCoords, string>();
        private readonly string _directory;

        protected HgtPathResolverCaching(string directory)
        {
            _directory = directory;
        }

        [NotNull]
        private string findPathForFile(HgtCellCoords coords)
        {
            string filename = coordsToFilename(coords);
            string[] potentialPaths =
            {
                Path.Combine(_directory, filename),
                Path.Combine(_directory, "Eurasia", filename),
                Path.Combine(_directory, "Africa", filename),
                Path.Combine(_directory, "South_America", filename),
                Path.Combine(_directory, "North_America", filename),
                Path.Combine(_directory, "Islands", filename),
            };

            var path = potentialPaths.FirstOrDefault(File.Exists);
            if (path != null) return path;

            var foundfile = new DirectoryInfo(_directory).EnumerateFiles(filename, SearchOption.AllDirectories)
                                                         .FirstOrDefault();
            if (foundfile != null) return foundfile.FullName;
            else throw new HgtFileNotFoundException(coords);
        }

        [NotNull]
        protected abstract string coordsToFilename(HgtCellCoords coords);

        public string FindFilePath(HgtCellCoords coords)
        {
            return _cache.GetOrAdd(coords, findPathForFile);
        }
    }
}
