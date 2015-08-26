using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace NSrtm.Core.Pgm.Grid
{
    public sealed class GridInMemory : GridBase
    {
        private readonly IReadOnlyList<UInt16> _pgmData;

        public GridInMemory([NotNull] MemoryStream pgmData, GridConstants pgmParameters)
            : base(pgmParameters)
        {
            if (pgmData == null) throw new ArgumentNullException("pgmData");
            _pgmData = getAsUint16(pgmData)
                .ToList()
                .AsReadOnly();
        }

        private IEnumerable<ushort> getAsUint16(MemoryStream pgmData)
        {
            var data = new List<UInt16>();
            using (EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Big, pgmData))
            {
                while (data.Count < PgmParameters.NumberOfPoints)
                {
                    data.Add(reader.ReadUInt16());
                }
            }
            return data;
        }

        protected override double getUndulationFrom(int pointPos)
        {
            var data = _pgmData[pointPos];
            return fromRawDataToUndulation(data);
        }
    }
}
