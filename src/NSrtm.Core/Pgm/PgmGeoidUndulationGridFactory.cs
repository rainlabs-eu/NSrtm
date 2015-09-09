using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MiscUtil.Conversion;
using MiscUtil.IO;
using NSrtm.Core.Pgm.DataDesciption;
using NSrtm.Core.Pgm.Exceptions;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "pgm", Justification = "PGM is a portable graymap shorcut")]
        public static IPgmGeoidUndulationGrid CreateGeoidUndulationGridInMemory(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".zip":
                    using (var zipArchive = ZipFile.OpenRead(filePath))
                    {
                        var entry = zipArchive.Entries.First(v => Path.GetExtension(v.Name) == ".pgm");
                        using (var zipStream = entry.Open())
                        {
                            return createGeoidUndulationGridInMemoryFromStream(zipStream);
                        }
                    }
                case ".pgm":
                    var stream = streamFromRaw(filePath);
                    return createGeoidUndulationGridInMemoryFromStream(stream);
                default:
                    throw new InvalidFileTypeException(String.Format("File extension {0} is not the right type, should be .zip or .pgm",
                                                                     Path.GetExtension(filePath)));
            }
        }

        private static IPgmGeoidUndulationGrid createGeoidUndulationGridInMemoryFromStream(Stream stream)
        {
            var dataDescription = PgmDataDescriptionExtractor.FromStream(stream);
            var scaledUndulation = readScaledUndulationFromStream(stream, dataDescription);
            return new PgmGeoidUndulationGridInMemory(scaledUndulation, dataDescription);
        }

        private static FileStream streamFromRaw(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static UInt16[] readScaledUndulationFromStream(Stream stream, PgmDataDescription dataDescription)
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
