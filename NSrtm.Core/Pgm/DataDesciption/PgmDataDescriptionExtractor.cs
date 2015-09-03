using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSrtm.Core.Pgm
{
    internal static class PgmPreamble
    {
        public static readonly IReadOnlyDictionary<string, Regex> Extractor =
            new Dictionary<string, Regex>
            {
                {"MagicNumber", new Regex(@"^(P5)[^$]")},
                {"Offset", new Regex(@"[\S\s]+(?=Offset)[^\d-]+(?<offset>-?\d+(\.\d+)?)")},
                {"Scale", new Regex(@"[\S\s]+(?=Scale)[^\d-]+(?<scale>-?\d+(\.\d+)?)")},
                {"LatitudeOrigin", new Regex(@"[\S\s]+(?=Origin)\D+(?<lat>\d+)")},
                {"LongitudeOrigin", new Regex(@"[\S\s]+(?=Origin)\D+\d+\D+(?<lon>\d+)")},
                {"WidthPoints", new Regex(@"(?<width>^\d+)", RegexOptions.Multiline)},
                {"HeightPoints", new Regex(@"^\d+\D+(?<height>\d+)", RegexOptions.Multiline)},
                {"MaxValue", new Regex(@"^\d[\S\s]+(?<maxValue>^\d+)$", RegexOptions.Multiline)},
            };
    }

    internal static class PgmDataDescriptionExtractor
    {
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
            var matches = PgmPreamble.Extractor.Select(kp => kp.Value.Match(preamble))
                                     .Zip(PgmPreamble.Extractor.Keys, (v, k) => new {k, v})
                                     .ToDictionary(x => x.k, x => x.v);
            foreach (var match in matches.Where(match => !match.Value.Success))
            {
                throw new ArgumentException(String.Format("Can not extract value {0} from the preamble", match.Key));
            }

            var offset = Convert.ToDouble(matches["Offset"].Groups["offset"].Value, CultureInfo.InvariantCulture);
            var scale = Convert.ToDouble(matches["Scale"].Groups["scale"].Value, CultureInfo.InvariantCulture);
            var lat = Convert.ToInt32(matches["LatitudeOrigin"].Groups["lat"].Value);
            var lon = Convert.ToInt32(matches["LongitudeOrigin"].Groups["lon"].Value);
            var width = Convert.ToInt32(matches["WidthPoints"].Groups["width"].Value);
            var height = Convert.ToInt32(matches["HeightPoints"].Groups["height"].Value);
            var maxValue = Convert.ToInt32(matches["MaxValue"].Groups["maxValue"].Value);

            return new PgmDataDescription(offset, scale, lat, lon, width, height, maxValue, preamble.Length);
        }
    }
}
