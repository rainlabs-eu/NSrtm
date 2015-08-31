using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.GridGraph
{
    public sealed class PgmGridGraphInMemory : PgmGridGraphBase
    {
        private readonly IReadOnlyList<UInt16> _pgmData;

        public PgmGridGraphInMemory([NotNull] IReadOnlyList<UInt16> pgmData, PgmGridGraphConstants pgmParameters)
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
