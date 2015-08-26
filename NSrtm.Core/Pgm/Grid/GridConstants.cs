namespace NSrtm.Core.Pgm.Grid
{
    public struct GridConstants
    {
        private readonly double _offset;
        private readonly double _scale;
        private readonly int _orginLat;
        private readonly int _orginLon;
        private readonly int _gridWidthPoints;
        private readonly int _gridHeightPoints;
        private readonly int _maxValue;

        public GridConstants(double offset, double scale, int orginLat, int orginLon, int gridWidthPoints, int gridHeightPoints, int maxValue)
        {
            _offset = offset;
            _scale = scale;
            _orginLat = orginLat;
            _orginLon = orginLon;
            _gridHeightPoints = gridHeightPoints;
            _gridWidthPoints = gridWidthPoints;
            _maxValue = maxValue;
        }

        public double Offset { get { return _offset; } }
        public double Scale { get { return _scale; } }
        public int OrginLat { get { return _orginLat; } }
        public int OrginLon { get { return _orginLon; } }
        public int GridWidthPoints { get { return _gridWidthPoints; } }
        public int GridHeightPoints { get { return _gridHeightPoints; } }
        public int MaxValue { get { return _maxValue; } }
        public int NumberOfPoints { get { return _gridHeightPoints * GridHeightPoints } }
    }
}
