using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.GridGraph
{
    public sealed class PgmGridGraphInMemory : IPgmGridGraph
    {
        private readonly UInt16[] _scaledUndulation;
        private readonly PgmDataDescription _pgmParameters;

        [CLSCompliantAttribute(false)]
        public PgmGridGraphInMemory([NotNull] UInt16[] scaledUndulation, PgmDataDescription pgmParameters)
        {
            if (scaledUndulation == null) throw new ArgumentNullException("scaledUndulation");
            _scaledUndulation = scaledUndulation;
            _pgmParameters = pgmParameters;
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly",
            Justification = "Simple range checking - it makes no sense to create a separate method")]
        private double getClosestUndulation(double latitude, double longitude)
        {
            int latPoints = (int)Math.Round((_pgmParameters.OriginLat - latitude) * _pgmParameters.LatitudeIncrement);
            int lonPoints = (int)Math.Round((longitude - _pgmParameters.OriginLon) * _pgmParameters.LongitudeIncrement);
            int closestPosition = (lonPoints + latPoints * _pgmParameters.GridGraphWidthPoints);

            if (closestPosition < 0 || closestPosition > _pgmParameters.NumberOfPoints)
            {
                throw new ArgumentOutOfRangeException("closestPosition");
            }
            var scaledUndulation = _scaledUndulation[closestPosition];
            return scaledUndulation.ToEgmFormat(Parameters);
        }

        public double GetUndulation(double latitude, double longitude)
        {
            return getClosestUndulation(latitude, longitude);;
        }

        public PgmDataDescription Parameters { get { return _pgmParameters; } }
    }
}
