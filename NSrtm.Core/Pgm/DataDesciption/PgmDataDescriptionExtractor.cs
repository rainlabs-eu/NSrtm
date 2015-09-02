using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NSrtm.Core.Pgm
{
    internal static class PgmDataDescriptionExtractor
    {
        private static readonly Regex _preambleParser =
            new Regex(
                @"^(P5)[\S\s]+(?=Offset)[^\d-]+(?<offset>-?\d+(\.\d+)?)[\S\s]+(?=Scale)[^\d-]+(?<scale>-?\d+(\.\d+)?)[\S\s]+(?=Origin)[^\d]+(?<lat>\d+)[^\d]+(?<lon>\d+)[^\d]+\S+\s(?<width>\d+)\D+(?<height>\d+)\D+(?<max>\d+)$");

        public static PgmDataDescription FromStream(Stream stream)
        {
            var preamble = extractPreambleFromStream(stream);
            return getConstatantsFromPreamble(preamble);
        }

        private static string extractPreambleFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 128, true))
            {
                var preamble = new StringBuilder(reader.ReadLine());
                string lastLine;
                do
                {
                    lastLine = reader.ReadLine();
                    preamble.Append(lastLine + "\n");
                } while (lastLine.StartsWith("#", StringComparison.CurrentCulture));
                preamble.Append(reader.ReadLine());
                return preamble.ToString();
            }
        }

        internal static PgmDataDescription getConstatantsFromPreamble(string preamble)
        {
            var match = _preambleParser.Match(preamble);
            if (!match.Success)
            {
                throw new ArgumentException("Preamble can not be recognized");
            }
            var offset = Convert.ToDouble(match.Groups["offset"].Value, CultureInfo.InvariantCulture);
            var scale = Convert.ToDouble(match.Groups["scale"].Value, CultureInfo.InvariantCulture);
            var lat = Convert.ToInt32(match.Groups["lat"].Value);
            var lon = Convert.ToInt32(match.Groups["lon"].Value);
            var width = Convert.ToInt32(match.Groups["width"].Value);
            var height = Convert.ToInt32(match.Groups["height"].Value);
            var maxValue = Convert.ToInt32(match.Groups["max"].Value);

            return new PgmDataDescription(offset, scale, lat, lon, width, height, maxValue, preamble.Length);
        }

    }
}
