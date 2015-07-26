using System;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    internal class HgtFileException : Exception
    {
        private readonly HgtCellCoords _coords;

        protected HgtFileException(HgtCellCoords coords, [NotNull] string message)
            : base(message)
        {
            _coords = coords;
        }

        public HgtFileException(HgtCellCoords coords, string message, Exception innerException)
            : base(message, innerException)
        {
            _coords = coords;
        }

        public HgtCellCoords Coords { get { return _coords; } }
    }
}
