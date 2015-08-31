using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSrtm.Core.Pgm.GridGraph
{
    public struct PgmGridGraphConstants
    {
        private readonly double _offset;
        private readonly double _scale;
        private readonly int _originLat;
        private readonly int _originLon;
        private readonly int _gridWidthPoints;
        private readonly int _gridHeightPoints;
        private readonly int _maxValue;
        private readonly int _preambleLength;
        private readonly int _numberOfPoints;
        private readonly double _latitudeIncrement;
        private readonly double _longitudeIncrement;

        public PgmGridGraphConstants(
            double offset,
            double scale,
            int originLat,
            int originLon,
            int gridWidthPoints,
            int gridHeightPoints,
            int maxValue,
            int preambleLength)
        {
            _offset = offset;
            _scale = scale;
            _originLat = originLat;
            _originLon = originLon;
            _gridHeightPoints = gridHeightPoints;
            _gridWidthPoints = gridWidthPoints;
            _maxValue = maxValue;
            _preambleLength = preambleLength;
            _numberOfPoints = _gridHeightPoints * _gridWidthPoints;
            _longitudeIncrement = _gridWidthPoints / 360.0;
            _latitudeIncrement = (_gridHeightPoints - 1) / 180.0;
        }

        public double Offset { get { return _offset; } }
        public double Scale { get { return _scale; } }
        public int OriginLat { get { return _originLat; } }
        public int OriginLon { get { return _originLon; } }
        public int GridWidthPoints { get { return _gridWidthPoints; } }
        public int GridHeightPoints { get { return _gridHeightPoints; } }
        public int MaxValue { get { return _maxValue; } }
        public int NumberOfPoints { get { return _numberOfPoints; } }
        public int PreambleLength { get { return _preambleLength; } }
        public double LatitudeIncrement { get { return _latitudeIncrement; } }
        public double LongitudeIncrement { get { return _longitudeIncrement; } }
    }

    internal static class PgmGridConstantsExtractor
    {
        private static readonly Regex LatLonParser = new Regex("^(?<deg>[-+0-9]+)+(?<pos>[ENSW]*)$");

        public static PgmGridGraphConstants FromStream(Stream stream)
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
                    throw new FileFormatException(String.Format("Wrong pgm magic number. Actual {0}, but should be {1}.", preamble, _pgmMagicNumber));
                }

                string lastLine;
                do
                {
                    lastLine = reader.ReadLine();
                    preamble += lastLine;
                    preamble += "\n";
                } while (lastLine.StartsWith("#"));
                preamble += reader.ReadLine();

                return preamble;
            }
        }

        private static PgmGridGraphConstants getConstatantsFromPreamble(string preamble) //TODO define magic numbers
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

            return new PgmGridGraphConstants(offset, scale, lat, lon, width, hight, maxValue, preamble.Length);
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
