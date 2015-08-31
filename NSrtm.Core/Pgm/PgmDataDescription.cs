using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSrtm.Core.Pgm
{
    public struct PgmDataDescription : IEquatable<PgmDataDescription>
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
        private readonly double _latitudeIncrement;
        private readonly double _longitudeIncrement;

        public PgmDataDescription(
            double offset,
            double scale,
            int originLat,
            int originLon,
            int gridGraphWidthPoints,
            int gridGraphHeightPoints,
            int maxValue,
            int preambleLength)
        {
            _offset = offset;
            _scale = scale;
            _originLat = originLat;
            _originLon = originLon;
            _gridGraphHeightPoints = gridGraphHeightPoints;
            _gridGraphWidthPoints = gridGraphWidthPoints;
            _maxValue = maxValue;
            _preambleLength = preambleLength;
            _numberOfPoints = _gridGraphHeightPoints * _gridGraphWidthPoints;
            _longitudeIncrement = _gridGraphWidthPoints / 360.0;
            _latitudeIncrement = (_gridGraphHeightPoints - 1) / 180.0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PgmDataDescription && Equals((PgmDataDescription)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _offset.GetHashCode();
                hashCode = (hashCode * 397) ^ _scale.GetHashCode();
                hashCode = (hashCode * 397) ^ _originLat;
                hashCode = (hashCode * 397) ^ _originLon;
                hashCode = (hashCode * 397) ^ _gridGraphWidthPoints;
                hashCode = (hashCode * 397) ^ _gridGraphHeightPoints;
                hashCode = (hashCode * 397) ^ _maxValue;
                hashCode = (hashCode * 397) ^ _preambleLength;
                return hashCode;
            }
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
        public double LatitudeIncrement { get { return _latitudeIncrement; } }
        public double LongitudeIncrement { get { return _longitudeIncrement; } }

        public bool Equals(PgmDataDescription other)
        {
            return _offset.Equals(other._offset) && _scale.Equals(other._scale) && _originLat == other._originLat && _originLon == other._originLon &&
                   _gridGraphWidthPoints == other._gridGraphWidthPoints && _gridGraphHeightPoints == other._gridGraphHeightPoints &&
                   _maxValue == other._maxValue && _preambleLength == other._preambleLength;
        }

        #region Static Members

        public static bool operator ==(PgmDataDescription left, PgmDataDescription right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PgmDataDescription left, PgmDataDescription right)
        {
            return !left.Equals(right);
        }

        #endregion
    }

    internal static class PgmRawDataConverter
    {
        public static double ToEgmFormat(this UInt16 rawData, PgmDataDescription pgmParameters)
        {
            if (rawData > pgmParameters.MaxValue)
            {
                throw new ArgumentOutOfRangeException("rawData");
            }
            return rawData * pgmParameters.Scale + pgmParameters.Offset;
        }
    }

    internal static class PgmDataDescriptionExtractor
    {
        private static readonly Regex LatLonParser = new Regex("^(?<deg>[-+0-9]+)+(?<pos>[ENSW]*)$");

        public static PgmDataDescription FromStream(Stream stream)
        {
            var preamble = extractPreambleFromStream(stream);
            return getConstatantsFromPreamble(preamble);
        }

        private static string extractPreambleFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
            {
                const string _pgmMagicNumber = "P5";
                var preamble = reader.ReadLine();
                if (preamble != _pgmMagicNumber)
                {
                    throw new FileFormatException(String.Format("Wrong portable gray map magic number. Actual {0}, but should be {1}.", preamble, _pgmMagicNumber));
                }

                string lastLine;
                do
                {
                    lastLine = reader.ReadLine();
                    preamble += lastLine;
                    preamble += "\n";
                } while (lastLine.StartsWith("#", StringComparison.CurrentCulture));
                preamble += reader.ReadLine();

                return preamble;
            }
        }

        private static PgmDataDescription getConstatantsFromPreamble(string preamble) //TODO define magic numbers
        {
            var unifyPreamble = preamble.Replace("#", "\n");
            var words = unifyPreamble.Split(' ', '\n')
                                     .ToList();
            var offset = Convert.ToDouble(words[getParameterIndexUsingName(words, "Offset")], CultureInfo.InvariantCulture);
            var scale = Convert.ToDouble(words[getParameterIndexUsingName(words, "Scale")], CultureInfo.InvariantCulture);
            var latIndex = getParameterIndexUsingName(words, "Origin");
            var lat = getLatLonValues(words, latIndex);
            var lon = getLatLonValues(words, latIndex + 1);
            var indexOfWidthInPreamble = words.Count - 4;
            var width = Convert.ToInt32(words[indexOfWidthInPreamble]);
            int indexOfHightInPreamble = words.Count - 2;
            var hight = Convert.ToInt32(words[indexOfHightInPreamble]);
            var maxValue = Convert.ToInt32(words.Last());

            return new PgmDataDescription(offset, scale, lat, lon, width, hight, maxValue, preamble.Length);
        }

        private static int getParameterIndexUsingName(List<string> words, string parameter)
        {
            return words.FindIndex(v => v == parameter) + 1;
        }

        private static int getLatLonValues(List<string> words, int index)
        {
            var coord = (words[index]);
            var match = LatLonParser.Match(coord);
            if (!match.Success)
            {
                throw new ArgumentException(string.Format("Lat/long value of '{0}' is not recognised", coord));
            }

            return int.Parse(match.Groups["deg"].Value);
        }
    }
}
