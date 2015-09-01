using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NSrtm.Core.Pgm
{
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
