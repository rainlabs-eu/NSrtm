using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.Grid
{
    public sealed class GridFromFile : GridBase
    {
        private readonly FileStream _file;
        private readonly Object _thisLock = new Object();

        public GridFromFile([NotNull] FileStream stream, GridConstants pgmParameters) : base(pgmParameters)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            _file = stream;
        }

        protected override double getUndulationFrom(int position)
        {
            lock (_thisLock)
            {
                _file.Seek(position + PgmParameters.PreambleLength, SeekOrigin.Begin);
                UInt16 rawData = (UInt16)(_file.ReadByte() << 8 | _file.ReadByte());
                return fromRawDataToUndulation(rawData);
            }
        }
    }
}
