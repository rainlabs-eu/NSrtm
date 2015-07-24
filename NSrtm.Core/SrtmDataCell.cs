using System;
using System.IO;

namespace NSrtm.Core
{
    /// <summary>
    ///     SRTM data cell.
    /// </summary>
    public class SrtmDataCell
    {
        private readonly byte[] _hgtData;
        private readonly int _pointsPerCell;
        private readonly int _latitudeOffset;
        private readonly int _longitudeOffset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SrtmDataCell" /> class.
        /// </summary>
        /// <param name='filepath'>
        ///     Filepath.
        /// </param>
        /// <exception cref='FileNotFoundException'>
        ///     Is thrown when a file path argument specifies a file that does not exist.
        /// </exception>
        /// <exception cref='ArgumentException'>
        ///     Is thrown when an argument passed to a method is invalid.
        /// </exception>
        public SrtmDataCell(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("File not found.", filepath);

            if (string.Compare(".hgt", Path.GetExtension(filepath), StringComparison.CurrentCultureIgnoreCase) != 0)
                throw new ArgumentException("Invalid extension.", "filepath");

            string filename = Path.GetFileNameWithoutExtension(filepath)
                                  .ToLower();
            string[] fileCoordinate = filename.Split('e', 'w');
            if (fileCoordinate.Length != 2)
                throw new ArgumentException("Invalid filename.", filepath);

            fileCoordinate[0] = fileCoordinate[0].TrimStart('n', 's');

            _latitudeOffset = int.Parse(fileCoordinate[0]);
            if (filename.Contains("s"))
                _latitudeOffset *= -1;

            _longitudeOffset = int.Parse(fileCoordinate[1]);
            if (filename.Contains("w"))
                _longitudeOffset *= -1;

            _hgtData = File.ReadAllBytes(filepath);

            switch (_hgtData.Length)
            {
                case 1201 * 1201 * 2: // SRTM-3
                    _pointsPerCell = 1201;
                    break;
                case 3601 * 3601 * 2: // SRTM-1
                    _pointsPerCell = 3601;
                    break;
                default:
                    throw new ArgumentException("Invalid file size.", filepath);
            }
        }

        public double GetHeight(double latitude, double longitude)
        {
            int localLat = (int)((latitude - _latitudeOffset) * _pointsPerCell);
            int localLon = (int)((longitude - _longitudeOffset) * _pointsPerCell);
            int bytesPos = ((_pointsPerCell - localLat - 1) * _pointsPerCell * 2) + localLon * 2;

            Console.WriteLine(bytesPos);

            if (bytesPos < 0 || bytesPos > _pointsPerCell * _pointsPerCell * 2)
                throw new ArgumentException("latitude or longitude out of range");

            // Motorola "big endian" order with the most significant byte first
            return (_hgtData[bytesPos]) << 8 | _hgtData[bytesPos + 1];
        }
    }
}
