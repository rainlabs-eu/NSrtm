using System;
using System.Collections.Generic;
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
                    PgmDataDescription dataDescription;
                    using (var zipStream = entry.Open())
                    {
                        dataDescription = PgmDataDescriptionExtractor.FromStream(zipStream);
                    }
                    using (var zipStream = entry.Open()) //Can not go back to the beginning of the file
                    {
                var scaledUndulation = getScaledUndulationFromPath(zipStream, dataDescription);
                return new PgmGeoidUndulationGridInMemory(scaledUndulation, dataDescription);
                    }
                }
            }
            else
            {
                var stream = streamFromRaw(filePath);
                var dataDescription = PgmDataDescriptionExtractor.FromStream(stream);
                stream.Position = 0;
                var scaledUndulation = getScaledUndulationFromPath(stream, dataDescription);
                return new PgmGeoidUndulationGridInMemory(scaledUndulation, dataDescription);
            }
        }

        private static FileStream streamFromRaw(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static UInt16[] getScaledUndulationFromPath(Stream stream, PgmDataDescription dataDescription)
        {
            var dataLength = dataDescription.NumberOfPoints;
            var data = new UInt16[dataLength];
            using (var binReader = new EndianBinaryReader(EndianBitConverter.Big, stream))
            {
                for (int i = 0; i < dataLength; i++)
                {
                    if (i > dataDescription.PreambleLength / 2)
                    {
                        data[i - dataDescription.PreambleLength / 2] = binReader.ReadUInt16();
                    }
                }
            }
            return data;
        }
    }
}
