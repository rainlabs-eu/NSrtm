using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MiscUtil.Conversion;
using MiscUtil.IO;
using NSrtm.Core.Pgm.GeoidUndulationGrid;

namespace NSrtm.Core.Pgm
{
    internal static class PgmGeoidUndulationGridFactory
    {
        public static IPgmGeoidUndulationGrid CreateGeoidUndulationGridInFile(string filePath)
        {
            var stream = streamFromRaw(filePath);
            var dataDescription = PgmDataDescriptionExtractor.FromStream(stream);
            return new PgmGeoidUndulationGridInFile(stream, dataDescription);
        }

        public static IPgmGeoidUndulationGrid CreateGeoidUndulationGridInMemory(string filePath)
        {
            var zipDirectory = Path.GetDirectoryName(Path.GetDirectoryName(filePath));
            if (Path.GetExtension(zipDirectory) == ".zip")
            {
                using (var zipArchive = ZipFile.OpenRead(zipDirectory))
                {
                    var entry = zipArchive.Entries.First(v => v.Name == Path.GetFileName(filePath));
                    using (var zipStream = entry.Open())
                    {
                        return createGeoidUndulationGridInMemoryFromStream(zipStream);
                    }
                }
            }
            else
            {
                var stream = streamFromRaw(filePath);
                return createGeoidUndulationGridInMemoryFromStream(stream);
            }
        }

        private static IPgmGeoidUndulationGrid createGeoidUndulationGridInMemoryFromStream(Stream stream)
        {
            var dataDescription = PgmDataDescriptionExtractor.FromStream(stream);
            var scaledUndulation = getScaledUndulationFromStream(stream, dataDescription);
            return new PgmGeoidUndulationGridInMemory(scaledUndulation, dataDescription);
        }

        private static FileStream streamFromRaw(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static UInt16[] getScaledUndulationFromStream(Stream stream, PgmDataDescription dataDescription)
        {
            var dataLength = dataDescription.NumberOfPoints;
            var data = new UInt16[dataLength];
            using (var binReader = new EndianBinaryReader(EndianBitConverter.Big, stream))
            {
                for (int i = 0; i < dataLength; i++)
                {
                    data[i] = binReader.ReadUInt16();
                }
            }
            return data;
        }
    }
}
