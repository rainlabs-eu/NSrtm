using System;
using JetBrains.Annotations;
using NSrtm.Core.Pgm.DataDesciption;

namespace NSrtm.Core.Pgm.GeoidUndulationGrid
{
    internal sealed class PgmGeoidUndulationGridInMemory : IPgmGeoidUndulationGrid
    {
        private readonly UInt16[] _scaledUndulation;
        private readonly PgmDataDescription _pgmParameters;

        public PgmGeoidUndulationGridInMemory([NotNull] UInt16[] scaledUndulation, PgmDataDescription pgmParameters)
        {
            if (scaledUndulation == null) throw new ArgumentNullException("scaledUndulation");
            _scaledUndulation = scaledUndulation;
            _pgmParameters = pgmParameters;
        }

        public double GetUndulation(double latitude, double longitude)
        {
            var closestPosition = PgmDataConverter.CoordinatesToClosestGridPosition(latitude, longitude, _pgmParameters);
            var scaledUndulation = _scaledUndulation[closestPosition];
            return scaledUndulation.RawToFinalFormat(_pgmParameters);
        }
    }
}
