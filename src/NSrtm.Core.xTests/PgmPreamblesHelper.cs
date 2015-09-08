namespace NSrtm.Core.xTests.Pgm
{
    internal static class PgmGeographicLibPreambles
    {
        public static readonly string Egm2008WithStep10 =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM2008, 1-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm2008
# DateTime 2009-08-31 06:54:00
# MaxBilinearError 0.025
# RMSBilinearError 0.001
# MaxCubicError 0.003
# RMSCubicError 0.001
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
21600  10801
65535";

        public static readonly string Egm2008WithStep25 =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM2008, 2.5-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm2008
# DateTime 2009-08-31 06:54:07
# MaxBilinearError 0.135
# RMSBilinearError 0.003
# MaxCubicError 0.031
# RMSCubicError 0.001
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
8640  4321
65535";

        public static readonly string Egm2008WithStep50 =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM2008, 5-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm2008
# DateTime 2009-08-29 18:45:00
# MaxBilinearError 0.478
# RMSBilinearError 0.012
# MaxCubicError 0.294
# RMSCubicError 0.005
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
4320    2161
65535";

        public static readonly string Egm96WithStep50 =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM96, 5-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm96/egm96.html
# DateTime 2009-08-29 18:45:03
# MaxBilinearError 0.140
# RMSBilinearError 0.005
# MaxCubicError 0.003
# RMSCubicError 0.001
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
4320 2161
65535";

        public static readonly string Egm96WithStep150 =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM96, 15-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm96/egm96.html
# DateTime 2009-08-29 18:45:02
# MaxBilinearError 1.152
# RMSBilinearError 0.040
# MaxCubicError 0.169
# RMSCubicError 0.007
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
1440 721
65535";

        public static readonly string Egm84WithStep150 =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM84, 15-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/wgs84_180/wgs84_180.html
# DateTime 2009-08-29 18:45:02
# MaxBilinearError 0.413
# RMSBilinearError 0.018
# MaxCubicError 0.020
# RMSCubicError 0.001
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
1440 721
65535";

        public static readonly string Egm84WithStep300 =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM84, 30-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/wgs84_180/wgs84_180.html
# DateTime 2009-08-29 18:45:02
# MaxBilinearError 1.546
# RMSBilinearError 0.070
# MaxCubicError 0.274
# RMSCubicError 0.014
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
720  361
65535";
    }

    internal static class PgmWrongPreambles
    {
        public static readonly string WithoutMagicNumber =
            @"# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM2008, 1-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm2008
# DateTime 2009-08-31 06:54:00
# MaxBilinearError 0.025
# RMSBilinearError 0.001
# MaxCubicError 0.003
# RMSCubicError 0.001
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
21600  10801
65535";

        public static readonly string WithWrongMagicNumber =
            @"P7
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM2008, 1-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm2008
# DateTime 2009-08-31 06:54:00
# MaxBilinearError 0.025
# RMSBilinearError 0.001
# MaxCubicError 0.003
# RMSCubicError 0.001
# Offset -108
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
21600  10801
65535";

        public static readonly string WithMissingFields =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM96, 15-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm96/egm96.html
# DateTime 2009-08-29 18:45:02
# MaxBilinearError 1.152
# RMSBilinearError 0.040
# MaxCubicError 0.169
# RMSCubicError 0.007
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
1440 721
65535";
    }

    internal static class PgmAcceptablePreambles
    {
        public static readonly string WithChangedFieldsOrder =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM84, 30-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/wgs84_180/wgs84_180.html
# DateTime 2009-08-29 18:45:02
# RMSBilinearError 0.070
# Offset -108
# MaxBilinearError 1.546
# MaxCubicError 0.274
# RMSCubicError 0.014
# Scale 0.003
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
720  361
65535";

        public static readonly string WithChangedOffsetAndScaleFormat =
            @"P5
# Geoid file in PGM format for the GeographicLib::Geoid class
# Description WGS84 EGM2008, 5-minute grid
# URL http://earth-info.nga.mil/GandG/wgs84/gravitymod/egm2008
# DateTime 2009-08-29 18:45:00
# MaxBilinearError 0.478
# RMSBilinearError 0.012
# MaxCubicError 0.294
# RMSCubicError 0.005
# Offset +108.0
# Scale 5
# Origin 90N 0E
# AREA_OR_POINT Point
# Vertical_Datum WGS84
4320    2161
65535";
    }
}
