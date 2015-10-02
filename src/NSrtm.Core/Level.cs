using System;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    public enum Level
    {
        EllipsoidWgs84,
        GeoidEgm2008,
        GeoidEgm96,
        GeoidEgm84,
        Terrain
    }

    internal sealed class LevelExtensions
    {
        #region Static Members

        public static Level Parse([NotNull] string s)
        {
            if (s == null) throw new ArgumentNullException("s");
            switch (s)
            {
                case "2008":
                    return Level.GeoidEgm2008;
                case "96":
                    return Level.GeoidEgm96;
                case "84":
                    return Level.GeoidEgm84;
                default:
                    throw new FormatException(String.Format("{0} is not a defined value for enum type {1}", s, typeof(Level).FullName));
            }
        }

        #endregion
    }
}
