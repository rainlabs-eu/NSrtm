using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSrtm.Core.Pgm.DataDesciption
{
    internal static class PgmPreamble
    {
        public static readonly IReadOnlyDictionary<string, Regex> Extractor =
            new Dictionary<string, Regex>
            {
                {"MagicNumber", new Regex(@"^(P5)[^$]", RegexOptions.Singleline)},
                {"Offset", new Regex(@"^#[\s]Offset[\s]+(?<offset>.+)$", RegexOptions.Multiline)},
                {"Scale", new Regex(@"^#[\s]Scale[\s]+(?<scale>.+)$", RegexOptions.Multiline)},
                {"Origin", new Regex(@"^#[\s]Origin[\s](?<lat>\d+)(?<latPos>[NS])\s(?<lon>\d+)(?<lonPos>[EW])", RegexOptions.Multiline)},
                {"PointParameters", new Regex(@"(\n|\r|\r\n)(?<width>\d+)\s+(?<height>\d+)(\n|\r|\r\n)(?<maxValue>\d+)", RegexOptions.Multiline)},
            };
    }

    internal static class PgmDataDescriptionExtractor
    {
        public static PgmDataDescription FromStream(Stream stream)
        {
            var preamble = extractPreambleFromStream(stream);
            return getConstatantsFromPreamble(preamble);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison",
            MessageId = "System.String.StartsWith(System.String)",
            Justification = "In this case explicitly setting this parameter only reduces the readability of the code.")]
        private static string extractPreambleFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var preamble = new StringBuilder(reader.ReadLine());
                string lastLine;
                do
                {
                    lastLine = reader.ReadLine();
                    preamble.Append(lastLine + "\n");
                } while (lastLine.StartsWith("#"));
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

            var offset = Double.Parse(matches["Offset"].Groups["offset"].Value, CultureInfo.InvariantCulture);
            var scale = Double.Parse(matches["Scale"].Groups["scale"].Value, CultureInfo.InvariantCulture);
            var latAngle = Int32.Parse(matches["Origin"].Groups["lat"].Value);
            var lonAngle = Int32.Parse(matches["Origin"].Groups["lon"].Value);
            var width = Int32.Parse(matches["PointParameters"].Groups["width"].Value);
            var height = Int32.Parse(matches["PointParameters"].Groups["height"].Value);
            var maxValue = Int32.Parse(matches["PointParameters"].Groups["maxValue"].Value);

            var lat = matches["Origin"].Groups["latPos"].Value == "N" ? latAngle : -latAngle;
            var lon = matches["Origin"].Groups["lonPos"].Value == "E" ? lonAngle : lonAngle + 180;

            return new PgmDataDescription(offset, scale, lat, lon, width, height, maxValue, preamble.Length);
        }
    }

    internal static class BinaryReaderExtension
    {
        public static string ReadLine(this BinaryReader reader)
        {
            var result = new StringBuilder();
            char character;
            while ((character = reader.ReadChar()) != '\n')
            {
                if (character != '\r' && character != '\n')
                {
                    result.Append(character);
                }
            }

            return result.ToString();
        }
    }
}
