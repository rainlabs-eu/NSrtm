using System;

namespace NSrtm.Demo
{
    internal struct ElevationValueStats
    {
        public readonly float Min;
        public readonly float Max;
        public readonly float Range;
        public readonly float Average;
        private readonly TimeSpan _time;
        private readonly int _totalCount;
        private readonly int _validCount;

        public ElevationValueStats(float min, float max, float range, float average, TimeSpan time, int totalCount, int validCount)
        {
            Min = min;
            Max = max;
            Range = range;
            Average = average;
            _time = time;
            _totalCount = totalCount;
            _validCount = validCount;
        }

        public override string ToString()
        {
            return string.Format("Min: {0}, Max: {1}, Range: {2}, Average: {3}, Elapsed: {4}, Samples per second {5:e2}, Missing data {6:f3} %",
                                 Min,
                                 Max,
                                 Range,
                                 Average,
                                 _time,
                                 _totalCount / _time.TotalSeconds,
                                 (_totalCount - _validCount) * 100.0 / _totalCount);
        }
    }
}