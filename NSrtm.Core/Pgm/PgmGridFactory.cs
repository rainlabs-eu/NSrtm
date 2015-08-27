using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MiscUtil.Conversion;
using MiscUtil.IO;
using NSrtm.Core.Pgm.Grid;
using Ionic.Zip;
using ZipFile = Ionic.Zip.ZipFile;

namespace NSrtm.Core.Pgm
{
    public static class PgmGridFactory
    {
        public static IGrid CreateGridWithDataInMemory(string filePath)
        {
            if (filePath.Contains("zip")) //TODO
            {
                var zipDirectory = Path.GetDirectoryName(Path.GetDirectoryName(filePath));
                using (var zip = ZipFile.Read(zipDirectory))
                {
                    var entry = zip.Entries.First(v => Path.GetFileName(v.FileName) == Path.GetFileName(filePath));
                    var stream = new MemoryStream();
                    entry.Extract(stream);
                    stream.Position = 0;
                    var gridConst = GridParametersExtractor.FromStream(stream);
                    var rawData = getDataFromPath(stream, gridConst);
                    return new GridInMemory(rawData, gridConst);
                }
            }
            else
            {
                var stream = streamFromRaw(filePath);
                var gridConst = GridParametersExtractor.FromStream(stream);
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
                while (stream.Position <= 2 * parameters.NumberOfPoints + parameters.PreambleLength)
                {
                    data.Add(binReader.ReadUInt16());
                }
            }
            return data;
        }
    }
}
