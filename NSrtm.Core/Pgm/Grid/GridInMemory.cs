using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace NSrtm.Core.Pgm.Grid
{
    public sealed class GridInMemory : IGrid
    {
        private readonly GridConstants _pgmParameters;
        private readonly IReadOnlyList<UInt16> _pgmData;

        public GridInMemory([NotNull] MemoryStream pgmData, GridConstants pgmParameters)
        {
            if (pgmData == null) throw new ArgumentNullException("pgmData");
            _pgmParameters = pgmParameters;
            _pgmData = getAsUint16(pgmData)
                .ToList()
                .AsReadOnly();
        }

        private IEnumerable<ushort> getAsUint16(MemoryStream pgmData)
        {
            var data = new List<UInt16>();
            using (EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Big, pgmData))
            {
                while (data.Count < _pgmParameters.NumberOfPoints)
                {
                    data.Add(reader.ReadUInt16());
                }
            }
            return data;
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

        private double getDataFrom(int pointPos)
        {
            var rawData = _pgmData[pointPos];
            if (rawData > _pgmParameters.MaxValue)
            {
                throw new ArgumentOutOfRangeException("rawData");
            }
            return rawData * _pgmParameters.Scale + _pgmParameters.Offset;
        }

        public double GetClosestData(double latitude, double longitude)
        {
            int position = getClosestDataPosition(latitude, longitude);

            return getDataFrom(position);
        }

        public GridConstants Parameters { get { return _pgmParameters; } }
    }
}
