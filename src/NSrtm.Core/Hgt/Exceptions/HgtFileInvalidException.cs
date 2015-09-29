namespace NSrtm.Core
{
    internal class HgtFileInvalidException : HgtFileException
    {
        public HgtFileInvalidException(HgtCellCoords coords, string reason)
            : base(coords, string.Format("Invalid file ({2}) for coordinates [{0}, {1}]", coords.Lat, coords.Lon, reason))
        {
        }
    }
}
