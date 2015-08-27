using System;
using System.IO;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.Grid
{
    public sealed class PgmGridInFile : PgmGridBase
    {
        private readonly FileStream _fileStream;
        private readonly Object _thisLock = new Object();

        public PgmGridInFile([NotNull] FileStream stream, PgmGridConstants pgmParameters)
            : base(pgmParameters)
        {
            if (stream == null) throw new ArgumentNullException("path");
            _fileStream = stream;
        }

        protected override double getUndulationFrom(int position)
        {
            lock (_thisLock)
            {
                var offset = 2 * position + PgmParameters.PreambleLength + 2;
                _fileStream.Seek(offset, SeekOrigin.Begin);
                UInt16 rawData = (UInt16)(_fileStream.ReadByte() << 8 | _fileStream.ReadByte());
                return fromRawDataToUndulation(rawData);
            }
        }
    }
}
