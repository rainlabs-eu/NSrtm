using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MiscUtil.Conversion;
using MiscUtil.IO;
using NSrtm.Core.Pgm.Grid;

namespace NSrtm.Core.Pgm
{
    public static class PgmGridFactory
    {
        public static IGrid CreateGridWithDataInMemory(string filePath)
        {
            if (filePath.Contains("zip")) //TODO find extensions
            {
                var zipDirectory = Path.GetDirectoryName(Path.GetDirectoryName(filePath));
                using (var zipArchive = ZipFile.OpenRead(zipDirectory))
                {
                    var entry = zipArchive.Entries.First(v => v.Name == Path.GetFileName(filePath));
                    GridConstants gridConst;
                    using (var zipStream = entry.Open())
                    {
                        gridConst = GridParametersExtractor.FromStream(zipStream);
                    }
                    using (var zipStream = entry.Open())
                    {
                        var rawData = getDataFromPath(zipStream, gridConst);
                        return new GridInMemory(rawData, gridConst);
                    }
                }
            }
            else
            {
                var stream = streamFromRaw(filePath);
                var gridConst = GridParametersExtractor.FromStream(stream);
                stream.Position = 0;
                var rawData = getDataFromPath(stream, gridConst);
                return new GridInMemory(rawData, gridConst);
            }
        }

        private static Stream streamFromRaw(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static IGrid CreateGridDirectAccess(string filePath)
        {
            var gridConst = GridParametersExtractor.FromStream(streamFromRaw(filePath));
            return new GridFromFile(filePath, gridConst);
        }

        private static List<ushort> getDataFromPath(Stream stream, GridConstants parameters)
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
