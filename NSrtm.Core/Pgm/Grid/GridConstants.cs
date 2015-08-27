using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        private readonly int _preambleLength;
        private readonly int _numberOfPoints;

        public GridConstants(
            double offset,
            double scale,
            int orginLat,
            int orginLon,
            int gridWidthPoints,
            int gridHeightPoints,
            int maxValue,
            int preambleLength)
        {
            _offset = offset;
            _scale = scale;
            _orginLat = orginLat;
            _orginLon = orginLon;
            _gridHeightPoints = gridHeightPoints;
            _gridWidthPoints = gridWidthPoints;
            _maxValue = maxValue;
            _preambleLength = preambleLength;
            _numberOfPoints = _gridHeightPoints * _gridWidthPoints;
        }

        public double Offset { get { return _offset; } }
        public double Scale { get { return _scale; } }
        public int OrginLat { get { return _orginLat; } }
        public int OrginLon { get { return _orginLon; } }
        public int GridWidthPoints { get { return _gridWidthPoints; } }
        public int GridHeightPoints { get { return _gridHeightPoints; } }
        public int MaxValue { get { return _maxValue; } }
        public int NumberOfPoints { get { return _numberOfPoints; } }
        public int PreambleLength { get { return _preambleLength; } }
    }

    internal static class GridParametersExtractor
    {
        private
            const string _pgmMagicNumber = "P5";

        private static readonly Regex LatLonParser = new Regex("^(?<deg>[-+0-9]+)+(?<pos>[ENSW]*)$");

        public static GridConstants FromPath(string path)
        {
            var preamble = extractPreambleFromPath(path);
            return getConstatantsFromPreamble(preamble);
        }

        private static string extractPreambleFromPath(string path)
        {
            string preamble;
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream))
            {
                preamble = reader.ReadLine();
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
            }
            return preamble;
        }

        private static GridConstants getConstatantsFromPreamble(string preamble)
        {
            var unifyPreamble = preamble.Replace("#", "\n");
            var words = unifyPreamble.Split(' ', '\n')
                                     .ToList();
            var offset = Convert.ToDouble(words[getParameterIndexUsingName(words, "Offset")], CultureInfo.InvariantCulture);
            var scale = Convert.ToDouble(words[getParameterIndexUsingName(words, "Scale")], CultureInfo.InvariantCulture);
            var latIndex = getParameterIndexUsingName(words, "Origin");
            var lat = getLatLon(words, latIndex);
            var lon = getLatLon(words, latIndex + 1);
            var indexOfWidthInPreamble = words.Count - 4;
            var width = Convert.ToInt32(words[indexOfWidthInPreamble]);
            int indexOfHightInPreamble = words.Count - 2;
            var hight = Convert.ToInt32(words[indexOfHightInPreamble]);
            var maxValue = Convert.ToInt32(words.Last());

            return new GridConstants(offset, scale, lat, lon, width, hight, maxValue, preamble.Length);
        }

        private static int getParameterIndexUsingName(List<string> words, string parameter)
        {
            return words.FindIndex(v => v == parameter) + 1;
        }

        private static int getLatLon(List<string> words, int index)
        {
            var coord = (words[index]);
            var match = LatLonParser.Match(coord);
            if (!match.Success)
            {
                throw new ArgumentException(string.Format("Lat/long value of '{0}' is not recognised", coord));
            }

            var deg = int.Parse(match.Groups["deg"].Value);
            if (match.Groups["pos"].Value[0] == 'S')
            {
                deg *= -1;
            }
            return deg;
        }
    }
}
