using System;
using System.IO;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.GridGraph
{
    internal sealed class PgmGridGraphInFile : IPgmGridGraph, IDisposable
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
                return rawData.RawToEgmFormat(_pgmParameters);
            }
        }

        public void Dispose()
        {
            _fileStream.Dispose();
        }

        public double GetUndulation(double latitude, double longitude)
        {
            int closestPosition = PgmDataConverter.CoordinatesToClosestGridPosition(latitude, longitude, _pgmParameters);
            var closestPositionInRawData = 2 * closestPosition + _pgmParameters.PreambleLength + 2;
            return getUndulationFrom(closestPositionInRawData);
        }
    }
}
