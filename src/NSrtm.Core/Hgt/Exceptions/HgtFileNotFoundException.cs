namespace NSrtm.Core
{
    internal class HgtFileNotFoundException : HgtFileException
    {
        public HgtFileNotFoundException(HgtCellCoords coords)
            : base(coords, string.Format("Cannot find file for coordinates [{0}, {1}]", coords.Lat, coords.Lon))
        {
        }
    }
}
