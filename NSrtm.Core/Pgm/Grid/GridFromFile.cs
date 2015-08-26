using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core.Pgm.Grid
{
    public sealed class GridFromFile : IGrid
    {
        private readonly FileStream _file;
        private readonly GridConstants _parameters;
        private readonly Object _thisLock = new Object();

        public GridFromFile([NotNull] FileStream stream, GridConstants parameters)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            _parameters = parameters;
            _file = stream;
        }

        public double GetClosestData(double latitude, double longitude)
        {
            var position = getClosestDataPosition(latitude, longitude);
            return getDataFrom(position);
        }

        private int getClosestDataPosition(double latitude, double longitude)
        {
            var lonStep = _pgmParameters.GridWidthPoints / 360.0;
            var latStep = (_pgmParameters.GridHeightPoints - 1) / 180.0;

            int latPoints = (int)Math.Round((_pgmParameters.OrginLat - latitude) * latStep);
            int lonPoints = (int)Math.Round((longitude - _pgmParameters.OrginLon) * lonStep);
            int dataPosition = (lonPoints + latPoints * _pgmParameters.GridWidthPoints);

            if (dataPosition < 0 || dataPosition > _pgmParameters.NumberOfPoints)
                throw new ArgumentOutOfRangeException("dataPosition");
            return dataPosition;
        }

        private double getDataFrom(int position)
        {
            lock (_thisLock)
            {
                _file.Seek(position + _parameters.SkippedBytes, SeekOrigin.Begin);
                UInt16 rawElevation = (UInt16)(_file.ReadByte() << 8 | _file.ReadByte());
                if (rawElevation > _parameters.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(rawElevation.ToString());
                }
                return rawElevation * _parameters.Scale + _parameters.Offset;
            }
        }

        public GridConstants Parameters { get { return _parameters; } }
    }
}
