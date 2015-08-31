using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.GridGraph
{
    public sealed class PgmGridGraphInMemory : PgmGridGraphBase
    {
        private readonly double[] _scaledUndulation;

        public PgmGridGraphInMemory([NotNull] double[] scaledUndulation, PgmDataDescription pgmParameters)
            : base(pgmParameters)
        {
            if (scaledUndulation == null) throw new ArgumentNullException("scaledUndulation");
            _scaledUndulation = scaledUndulation;
        }

        protected override double getUndulationFrom(int pointPos)
        {
            return _scaledUndulation[pointPos];
        }
    }
}
