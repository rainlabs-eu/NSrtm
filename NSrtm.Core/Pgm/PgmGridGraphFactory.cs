using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MiscUtil.Conversion;
using MiscUtil.IO;
using NSrtm.Core.Pgm.GridGraph;

namespace NSrtm.Core.Pgm
{
    public static class PgmGridGraphFactory
    {
        public static IPgmGridGraph CreateGridInFile(string filePath)
        {
            var stream = (FileStream)streamFromRaw(filePath);
            var gridConst = PgmGridConstantsExtractor.FromStream(stream);
            return new PgmGridGraphInFile(stream, gridConst);
        }

        public static IPgmGridGraph CreateGridInMemory(string filePath)
        {
            var zipDirectory = Path.GetDirectoryName(Path.GetDirectoryName(filePath));
            if (Path.GetExtension(zipDirectory) == "zip")
            {
                using (var zipArchive = ZipFile.OpenRead(zipDirectory))
                {
                    var entry = zipArchive.Entries.First(v => v.Name == Path.GetFileName(filePath));
                    PgmGridGraphConstants pgmGridGraphConst;
                    using (var zipStream = entry.Open())
                    {
                        pgmGridGraphConst = PgmGridConstantsExtractor.FromStream(zipStream);
                    }
                    using (var zipStream = entry.Open()) //Can not go back to the beginning of the file
                    {
                        var rawData = getDataFromPath(zipStream, pgmGridGraphConst)
                            .AsReadOnly();
                        return new PgmGridGraphInMemory(rawData, pgmGridGraphConst);
                    }
                }
            }
            else
            {
                var stream = streamFromRaw(filePath);
                var gridConst = PgmGridConstantsExtractor.FromStream(stream);
                stream.Position = 0;
                var rawData = getDataFromPath(stream, gridConst)
                    .AsReadOnly();
                return new PgmGridGraphInMemory(rawData, gridConst);
            }
        }

        private static Stream streamFromRaw(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static List<ushort> getDataFromPath(Stream stream, PgmGridGraphConstants parameters)
        {
            var data = new List<UInt16>();
            using (var binReader = new EndianBinaryReader(EndianBitConverter.Big, stream))
            {
                for (int i = 0; i < parameters.NumberOfPoints + parameters.PreambleLength / 2; i++)
                {
                    data.Add(binReader.ReadUInt16());
                }
            }
            return data.Skip(parameters.PreambleLength)
                       .ToList();
        }
    }
}
