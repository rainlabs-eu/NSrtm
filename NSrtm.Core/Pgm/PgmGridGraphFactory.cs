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
        public static IPgmGridGraph CreateGridGraphInFile(string filePath)
        {
            var stream = (FileStream)streamFromRaw(filePath);
            var gridConst = PgmDataDescriptionExtractor.FromStream(stream);
            return new PgmGridGraphInFile(stream, gridConst);
        }

        public static IPgmGridGraph CreateGridGraphInMemory(string filePath)
        {
            var zipDirectory = Path.GetDirectoryName(Path.GetDirectoryName(filePath));
            if (Path.GetExtension(zipDirectory) == ".zip")
            {
                using (var zipArchive = ZipFile.OpenRead(zipDirectory))
                {
                    var entry = zipArchive.Entries.First(v => v.Name == Path.GetFileName(filePath));
                    PgmDataDescription pgmGridGraphConst;
                    using (var zipStream = entry.Open())
                    {
                        pgmGridGraphConst = PgmDataDescriptionExtractor.FromStream(zipStream);
                    }
                    using (var zipStream = entry.Open()) //Can not go back to the beginning of the file
                    {
                var scaledUndulation = getScaledUndulationFromPath(zipStream, pgmGridGraphConst);
                return new PgmGridGraphInMemory(scaledUndulation, pgmGridGraphConst);
                    }
                }
            }
            else
            {
                var stream = streamFromRaw(filePath);
                var gridConst = PgmDataDescriptionExtractor.FromStream(stream);
                stream.Position = 0;
                var scaledUndulation = getScaledUndulationFromPath(stream, gridConst);
                return new PgmGridGraphInMemory(scaledUndulation, gridConst);
            }
        }

        private static Stream streamFromRaw(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static UInt16[] getScaledUndulationFromPath(Stream stream, PgmDataDescription parameters)
        {
            var dataLength = parameters.NumberOfPoints;
            var data = new UInt16[dataLength];
            using (var binReader = new EndianBinaryReader(EndianBitConverter.Big, stream))
            {
                for (int i = 0; i < dataLength; i++)
                {
                    if (i > parameters.PreambleLength / 2)
                    {
                        data[i - parameters.PreambleLength / 2] = binReader.ReadUInt16();
                    }
                }
            }
            return data;
        }
    }
}
