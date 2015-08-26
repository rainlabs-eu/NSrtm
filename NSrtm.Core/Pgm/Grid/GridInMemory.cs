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

        public GridInMemory([NotNull] List<UInt16> pgmData, GridConstants pgmParameters)
            : base(pgmParameters)
        {
            if (pgmData == null) throw new ArgumentNullException("pgmData");
            _pgmData = pgmData.AsReadOnly();
        }

        protected override double getUndulationFrom(int pointPos)
        {
            var data = _pgmData[pointPos];
            return fromRawDataToUndulation(data);
        }
    }
}
