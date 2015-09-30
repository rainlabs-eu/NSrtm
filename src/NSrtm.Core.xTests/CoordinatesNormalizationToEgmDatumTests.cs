using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NSrtm.Core.Pgm;
using NSrtm.Core.Pgm.DataDesciption;
using NSrtm.Core.xTests.Pgm;
using Xunit;

namespace NSrtm.Core.xTests
{
    public class CoordinatesNormalizationToEgmDatumTests
    {
        [Theory]
        [MemberData("NormalizedCoordinates")]
        internal void NormalizationWorksCorrectlyForNormalizedData(PgmCellCoords inputCoords, PgmCellCoords expectedCoords)
        {
            var actual = PgmElevationProvider.normalizeCoords(inputCoords.Lat, inputCoords.Lon);
            AssertDeep.Equal(actual, expectedCoords, config => config.DoublePrecision = 1e-5);
        }

        [Theory]
        [MemberData("NonNormalizedCoordinates")]
        internal void NormalizationWorksCorrectlyForNonNormalizedData(PgmCellCoords inputCoords, PgmCellCoords expectedCoords)
        {
            var actual = PgmElevationProvider.normalizeCoords(inputCoords.Lat, inputCoords.Lon);
            AssertDeep.Equal(actual, expectedCoords, config => config.DoublePrecision = 1e-5);
        }

        public static IReadOnlyList<object[]> NonNormalizedCoordinates
        {
            get
            {
                return new List<object[]>
                       {
                           new object[] {new PgmCellCoords(0, -20), new PgmCellCoords(0, 340)},
                           new object[] {new PgmCellCoords(0, 361), new PgmCellCoords(0, 1)},
                           new object[] {new PgmCellCoords(100, 0), new PgmCellCoords(80, 180)},
                           new object[] {new PgmCellCoords(-100, 0), new PgmCellCoords(-80, 180)},
                           new object[] {new PgmCellCoords(100, -20), new PgmCellCoords(80, 160)},
                           new object[] {new PgmCellCoords(-91, 362), new PgmCellCoords(-89, 182)},
                       };
            }
        }

        public static IReadOnlyList<object[]> NormalizedCoordinates
        {
            get
            {
                return new List<object[]>
                       {
                           new object[] {new PgmCellCoords(0, 0), new PgmCellCoords(0, 0)},
                           new object[] {new PgmCellCoords(-90, 0), new PgmCellCoords(-90, 0)},
                           new object[] {new PgmCellCoords(90, 0), new PgmCellCoords(90, 0)},
                           new object[] {new PgmCellCoords(-90, 359), new PgmCellCoords(-90, 359)},
                           new object[] {new PgmCellCoords(-70, 35), new PgmCellCoords(-70, 35)},
                           new object[] {new PgmCellCoords(30, 100), new PgmCellCoords(30, 100)},
                       };
            }
        }
    }
}
