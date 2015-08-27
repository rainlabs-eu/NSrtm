using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiscUtil.Conversion;
using MiscUtil.IO;
using NSrtm.Core.Pgm.Grid;

namespace NSrtm.Core.Pgm
{
    public static class PgmGridFactory
    {
        public static IGrid CreateGridWithDataInMemory(string directory, string fileName)
        {
            var path = @"C:\mc\EGS2008ZIP\geoids\egm2008-2_5.pgm";
            var gridConst = GridParametersExtractor.FromPath(path);
            var rawData = getDataFromPath(path, gridConst);
            return new GridInMemory(rawData, gridConst);
        }

        public static IGrid CreateGridDirectAccess(string directory, string fileName)
        {
            var path = @"C:\mc\EGS2008ZIP\geoids\egm2008-2_5.pgm";
            var gridConst = GridParametersExtractor.FromPath(path);
            return new GridFromFile(path, gridConst);
        }

        private static List<ushort> getDataFromPath(string path, GridConstants parameters)
        {
            var data = new List<UInt16>();
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new EndianBinaryReader(EndianBitConverter.Big, stream))
            {
                while (reader.BaseStream.Position <= 2 * parameters.NumberOfPoints + parameters.PreambleLength)
                {
                    data.Add(reader.ReadUInt16());
                }
            }
            return data.Skip(parameters.PreambleLength).ToList();
        }
    }
}
