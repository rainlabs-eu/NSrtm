using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace NSrtm.Core.Pgm.Grid
{
    public sealed class PgmGridInMemory : PgmGridBase
    {
        private readonly IReadOnlyList<UInt16> _pgmData;

        public PgmGridInMemory([NotNull] IReadOnlyList<UInt16> pgmData, PgmGridConstants pgmParameters)
            : base(pgmParameters)
        {
            if (pgmData == null) throw new ArgumentNullException("pgmData");
            _pgmData = pgmData;
        }

        protected override double getUndulationFrom(int pointPos)
        {
            var data = _pgmData[pointPos];
            return fromRawDataToUndulation(data);
        }
    }
}
