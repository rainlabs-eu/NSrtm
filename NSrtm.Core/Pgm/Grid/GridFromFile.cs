using System;
using System.IO;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.Grid
{
    public sealed class GridFromFile : GridBase
    {
        private readonly FileStream _file;
        private readonly Object _thisLock = new Object();

        public GridFromFile([NotNull] string path, GridConstants pgmParameters)
            : base(pgmParameters)
        {
            if (path == null) throw new ArgumentNullException("path");
            _file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        protected override double getUndulationFrom(int position)
        {
            lock (_thisLock)
            {
                _file.Seek(2 * position + PgmParameters.PreambleLength + 2, SeekOrigin.Begin);
                UInt16 rawData = (UInt16)(_file.ReadByte() << 8 | _file.ReadByte());
                return fromRawDataToUndulation(rawData);
            }
        }
    }
}
