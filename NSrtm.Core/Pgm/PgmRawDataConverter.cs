using System;

namespace NSrtm.Core.Pgm
{
    internal static class PgmRawDataConverter
    {
        public static double ToEgmFormat(this UInt16 rawData, PgmDataDescription pgmParameters)
        {
            if (rawData > pgmParameters.MaxValue)
            {
                throw new ArgumentOutOfRangeException("rawData");
            }
            return rawData * pgmParameters.Scale + pgmParameters.Offset;
        }
    }
}
