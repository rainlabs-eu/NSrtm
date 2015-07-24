using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace NSrtm.Core
{
    public interface IHgtDataCell
    {
        double GetElevation(double latitude, double longitude);
    }

    internal class InvalidHgtDataCell : IHgtDataCell
    {
        private static readonly InvalidHgtDataCell invalid = new InvalidHgtDataCell();

        private InvalidHgtDataCell()
        {
        }

        public static InvalidHgtDataCell Invalid { get { return invalid; } }

        public double GetElevation(double latitude, double longitude)
        {
            return 0xffff;
        }
    }

    /// <summary>
    ///     SRTM data cell.
    /// </summary>
    public class HgtDataCell : IHgtDataCell
    {
        private const int srtm3Length = 1201 * 1201 * 2;
        private const int srtm1Length = 3601 * 3601 * 2;
        private readonly byte[] _hgtData;
        private readonly int _pointsPerCell;
        private readonly int _latitudeOffset;
        private readonly int _longitudeOffset;

        public HgtDataCell(byte[] hgtData, int pointsPerCell, int latitudeOffset, int longitudeOffset)
        {
            _hgtData = hgtData;
            _pointsPerCell = pointsPerCell;
            _latitudeOffset = latitudeOffset;
            _longitudeOffset = longitudeOffset;
        }

        public double GetElevation(double latitude, double longitude)
        {
            int localLat = (int)((latitude - _latitudeOffset) * _pointsPerCell);
            int localLon = (int)((longitude - _longitudeOffset) * _pointsPerCell);
            int bytesPos = ((_pointsPerCell - localLat - 1) * _pointsPerCell * 2) + localLon * 2;

            if (bytesPos < 0 || bytesPos > _pointsPerCell * _pointsPerCell * 2)
                throw new ArgumentException("latitude or longitude out of range");

            // Motorola "big endian" order with the most significant byte first
            return (_hgtData[bytesPos]) << 8 | _hgtData[bytesPos + 1];
        }

        public static HgtDataCell FromExistingHgtFile(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException(string.Format("File {0} not found.", filepath), filepath);

            if (!Path.HasExtension(filepath))
                throw new ArgumentException("Missing extension", "filepath");

            string extension = Path.GetExtension(filepath);
            if (string.Compare(".hgt", extension, StringComparison.CurrentCultureIgnoreCase) != 0)
                throw new ArgumentException(string.Format("Invalid extension {0}", extension), "filepath");

            string normalizedFilename = Path.GetFileNameWithoutExtension(filepath)
                                            .ToLower();

            int latitudeOffset;
            int longitudeOffset;
            LatitudeOffset(normalizedFilename, out latitudeOffset, out longitudeOffset);

            var hgtData = File.ReadAllBytes(filepath);
            if (hgtData.Length != srtm3Length && hgtData.Length != srtm1Length)
                throw new ArgumentException(string.Format("Invalid file size ({0} bytes)", hgtData.Length), "filepath");

            return fromDataAndOffsets(hgtData, latitudeOffset, longitudeOffset);
        }

        public static IHgtDataCell FromZipFileOrInvalid(string filepath)
        {
            if (!File.Exists(filepath)) return InvalidHgtDataCell.Invalid;

            return FromExistingZipFile(filepath);
        }

        public static HgtDataCell FromExistingZipFile(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException(string.Format("File {0} not found.", filepath), filepath);

            if (!Path.HasExtension(filepath))
                throw new ArgumentException("Missing extension", "filepath");

            string extension = Path.GetExtension(filepath);
            if (string.Compare(".zip", extension, StringComparison.CurrentCultureIgnoreCase) != 0)
                throw new ArgumentException(string.Format("Invalid extension {0}", extension), "filepath");

            string hgtFileName = Path.GetFileNameWithoutExtension(filepath)
                                     .ToLower();

            string innerExtension = Path.GetExtension(hgtFileName);
            if (string.Compare(".hgt", innerExtension, StringComparison.CurrentCultureIgnoreCase) != 0)
                throw new ArgumentException(string.Format("Invalid inner extension {0}", innerExtension), "filepath");

            string normalizedFilename = Path.GetFileNameWithoutExtension(hgtFileName)
                                            .ToLower();

            int latitudeOffset;
            int longitudeOffset;
            LatitudeOffset(normalizedFilename, out latitudeOffset, out longitudeOffset);

            ZipArchive zipArchive = ZipFile.OpenRead(filepath);
            var entry = zipArchive.Entries.Single();
            if (entry.Length != srtm3Length && entry.Length != srtm1Length)
                throw new ArgumentException(string.Format("Invalid file size ({0} bytes)", entry.Length), "filepath");

            byte[] hgtData;
            using (var zipStream = entry.Open())
            {
                using (var memory = new MemoryStream())
                {
                    zipStream.CopyTo(memory);
                    hgtData = memory.ToArray();
                }
            }
            return fromDataAndOffsets(hgtData, latitudeOffset, longitudeOffset);
        }

        private static HgtDataCell fromDataAndOffsets(byte[] hgtData, int latitudeOffset, int longitudeOffset)
        {
            int pointsPerCell;
            switch (hgtData.Length)
            {
                case srtm3Length: // SRTM-3
                    pointsPerCell = 1201;
                    break;
                case srtm1Length: // SRTM-1
                    pointsPerCell = 3601;
                    break;
                default:
                    throw new ArgumentException("hgtData");
            }

            return new HgtDataCell(hgtData, pointsPerCell, latitudeOffset, longitudeOffset);
        }

        private static void LatitudeOffset(string normalizedFilename, out int latitudeOffset, out int longitudeOffset)
        {
            string[] fileCoordinate = normalizedFilename.Split('e', 'w');
            if (fileCoordinate.Length != 2)
                throw new ArgumentException("Invalid filename.", normalizedFilename);

            fileCoordinate[0] = fileCoordinate[0].TrimStart('n', 's');

            latitudeOffset = int.Parse(fileCoordinate[0]);
            if (normalizedFilename.Contains("s"))
                latitudeOffset *= -1;

            longitudeOffset = int.Parse(fileCoordinate[1]);
            if (normalizedFilename.Contains("w"))
                longitudeOffset *= -1;
        }
    }
}
