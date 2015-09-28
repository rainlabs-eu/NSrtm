namespace NSrtm.Core.Pgm.DataDesciption
{
    internal struct PgmDataDescription
    {
        private readonly double _offset;
        private readonly double _scale;
        private readonly int _originLat;
        private readonly int _originLon;
        private readonly int _gridGraphWidthPoints;
        private readonly int _gridGraphHeightPoints;
        private readonly int _maxValue;
        private readonly int _preambleLength;
        private readonly int _numberOfPoints;
        private readonly double _latitudeIncrementDegrees;
        private readonly double _longitudeIncrementDegrees;
        private readonly Level _level;

        public PgmDataDescription(
            double offset,
            double scale,
            int originLat,
            int originLon,
            int gridGraphWidthPoints,
            int gridGraphHeightPoints,
            int maxValue,
            int preambleLength, Level level)
        {
            _offset = offset;
            _scale = scale;
            _originLat = originLat;
            _originLon = originLon;
            _gridGraphHeightPoints = gridGraphHeightPoints;
            _gridGraphWidthPoints = gridGraphWidthPoints;
            _maxValue = maxValue;
            _preambleLength = preambleLength;
            _level = level;
            _numberOfPoints = _gridGraphHeightPoints * _gridGraphWidthPoints;
            _longitudeIncrementDegrees = 360.0 / _gridGraphWidthPoints;
            _latitudeIncrementDegrees = 180.0 / (_gridGraphHeightPoints - 1);
        }

        public double Offset { get { return _offset; } }
        public double Scale { get { return _scale; } }
        public int OriginLat { get { return _originLat; } }
        public int OriginLon { get { return _originLon; } }
        public int GridGraphWidthPoints { get { return _gridGraphWidthPoints; } }
        public int GridGraphHeightPoints { get { return _gridGraphHeightPoints; } }
        public int MaxValue { get { return _maxValue; } }
        public int NumberOfPoints { get { return _numberOfPoints; } }
        public int PreambleLength { get { return _preambleLength; } }
        public double LatitudeIncrementDegrees { get { return _latitudeIncrementDegrees; } }
        public double LongitudeIncrementDegrees { get { return _longitudeIncrementDegrees; } }
        public Level Level { get { return _level; } }
    }
}
