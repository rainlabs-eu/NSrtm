using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.GridGraph
{
    public sealed class PgmGridGraphInMemory : PgmGridGraphBase
    {
        private readonly UInt16[] _scaledUndulation;

        public PgmGridGraphInMemory([NotNull] UInt16[] scaledUndulation, PgmDataDescription pgmParameters)
            : base(pgmParameters)
        {
            if (scaledUndulation == null) throw new ArgumentNullException("scaledUndulation");
            _scaledUndulation = scaledUndulation;
        }

        protected override double getUndulationFrom(int pointPos)
        {
            var scaledUndulation = _scaledUndulation[pointPos];
            return scaledUndulation.ToEgmFormat(PgmParameters);
        }
    }
}
