using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.GridGraph
{
    public sealed class PgmGridGraphInFile : IPgmGridGraph, IDisposable
    {
        private readonly FileStream _fileStream;
        private readonly Object _thisLock = new Object();
        private readonly PgmDataDescription _pgmParameters;

        public PgmGridGraphInFile([NotNull] FileStream stream, PgmDataDescription pgmParameters)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            _fileStream = stream;
            _pgmParameters = pgmParameters;
        }

        private double getUndulationFrom(int position)
        {
            lock (_thisLock)
            {
                _fileStream.Seek(position, SeekOrigin.Begin);
                UInt16 rawData = (UInt16)(_fileStream.ReadByte() << 8 | _fileStream.ReadByte());
                return rawData.ToEgmFormat(Parameters);
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly",
            Justification = "Simple range checking - it makes no sense to create a separate method")]
        private int getClosestPosition(double latitude, double longitude)
        {
            int latPoints = (int)Math.Round((_pgmParameters.OriginLat - latitude) * _pgmParameters.LatitudeIncrement);
            int lonPoints = (int)Math.Round((longitude - _pgmParameters.OriginLon) * _pgmParameters.LongitudeIncrement);
            int closestPosition = (lonPoints + latPoints * _pgmParameters.GridGraphWidthPoints);

            if (closestPosition < 0 || closestPosition > _pgmParameters.NumberOfPoints)
                throw new ArgumentOutOfRangeException("closestPosition");
            var closestPositionRawData = 2 * closestPosition + Parameters.PreambleLength + 2;
            return closestPositionRawData;
        }

        public void Dispose()
        {
            _fileStream.Dispose();
        }

        public PgmDataDescription Parameters { get { return _pgmParameters; } }

        public double GetUndulation(double latitude, double longitude)
        {
            var position = getClosestPosition(latitude, longitude);
            return getUndulationFrom(position);
        }
    }
}
