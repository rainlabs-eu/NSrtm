using System;

namespace NSrtm.Core
{
    public class HgtFileException : Exception
    {
        private readonly HgtCellCoords _coords;

        public HgtFileException(HgtCellCoords coords, string message)
            : base(message)
        {
        }

        public HgtFileException(HgtCellCoords coords, string message, Exception innerException)
            : base(message, innerException)
        {
            _coords = coords;
        }

        public HgtCellCoords Coords { get { return _coords; } }
    }
}
