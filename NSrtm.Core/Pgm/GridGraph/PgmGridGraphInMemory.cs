using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.GridGraph
{
    internal sealed class PgmGridGraphInMemory : IPgmGridGraph
    {
        private readonly UInt16[] _scaledUndulation;
        private readonly PgmDataDescription _pgmParameters;

        public PgmGridGraphInMemory([NotNull] UInt16[] scaledUndulation, PgmDataDescription pgmParameters)
        {
            if (scaledUndulation == null) throw new ArgumentNullException("scaledUndulation");
            _scaledUndulation = scaledUndulation;
            _pgmParameters = pgmParameters;
        }

        public double GetUndulation(double latitude, double longitude)
        {
            var closestPosition = PgmDataConverter.CoordinatesToClosestGridPosition(latitude, longitude, _pgmParameters);
            var scaledUndulation = _scaledUndulation[closestPosition];
            return scaledUndulation.RawToEgmFormat(_pgmParameters);
        }
    }
}
