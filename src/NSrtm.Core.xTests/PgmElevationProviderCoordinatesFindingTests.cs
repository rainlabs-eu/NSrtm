using System.Collections.Generic;
using NSrtm.Core.Pgm;
using Xunit;

namespace NSrtm.Core.xTests
{
    public class PgmElevationProviderCoordinatesFindingTests
    {
        [Theory]
        [MemberData("SimplePgmCellCoords")]
        internal void ExtractingValuesFromSimpleSegmentsWorksCorrectly(
            PgmCellCoords coords,
            double latIncrement,
            double lonIncrement,
            IEnumerable<PgmCellCoords> expected)
        {
            var actual = PgmElevationProvider.findCellAndSurroundingNodesCoords(coords, latIncrement, lonIncrement);
            AssertDeep.Equal(actual, expected);
        }

        [Theory]
        [MemberData("CornerPgmCellCoords")]
        internal void ExtractingValuesFromCornerSegmentsWorksCorrectly(
            PgmCellCoords coords,
            double latIncrement,
            double lonIncrement,
            IEnumerable<PgmCellCoords> expected)
        {
            var actual = PgmElevationProvider.findCellAndSurroundingNodesCoords(coords, latIncrement, lonIncrement);
            AssertDeep.Equal(actual, expected);
        }

        #region Static Members

        public static IReadOnlyList<object[]> SimplePgmCellCoords
        {
            get
            {
                return new List<object[]>
                       {
                           new object[]
                           {
                               new PgmCellCoords(10, 20),
                               1,
                               1,
                               new List<PgmCellCoords>
                               {
                                   new PgmCellCoords(9, 19),
                                   new PgmCellCoords(9, 20),
                                   new PgmCellCoords(9, 21),
                                   new PgmCellCoords(9, 22),
                                   new PgmCellCoords(10, 19),
                                   new PgmCellCoords(10, 20),
                                   new PgmCellCoords(10, 21),
                                   new PgmCellCoords(10, 22),
                                   new PgmCellCoords(11, 19),
                                   new PgmCellCoords(11, 20),
                                   new PgmCellCoords(11, 21),
                                   new PgmCellCoords(11, 22),
                                   new PgmCellCoords(12, 19),
                                   new PgmCellCoords(12, 20),
                                   new PgmCellCoords(12, 21),
                                   new PgmCellCoords(12, 22),
                               },
                           },
                           new object[]
                           {
                               new PgmCellCoords(-60, 60),
                               1,
                               1,
                               new List<PgmCellCoords>
                               {
                                   new PgmCellCoords(-61, 59),
                                   new PgmCellCoords(-61, 60),
                                   new PgmCellCoords(-61, 61),
                                   new PgmCellCoords(-61, 62),
                                   new PgmCellCoords(-60, 59),
                                   new PgmCellCoords(-60, 60),
                                   new PgmCellCoords(-60, 61),
                                   new PgmCellCoords(-60, 62),
                                   new PgmCellCoords(-59, 59),
                                   new PgmCellCoords(-59, 60),
                                   new PgmCellCoords(-59, 61),
                                   new PgmCellCoords(-59, 62),
                                   new PgmCellCoords(-58, 59),
                                   new PgmCellCoords(-58, 60),
                                   new PgmCellCoords(-58, 61),
                                   new PgmCellCoords(-58, 62),
                               },
                           },
                       };
            }
        }

        public static IReadOnlyList<object[]> CornerPgmCellCoords
        {
            get
            {
                return new List<object[]>
                       {
                           new object[]
                           {
                               new PgmCellCoords(0, 0),
                               1,
                               1,
                               new List<PgmCellCoords>
                               {
                                   new PgmCellCoords(-1, 359),
                                   new PgmCellCoords(-1, 0),
                                   new PgmCellCoords(-1, 1),
                                   new PgmCellCoords(-1, 2),
                                   new PgmCellCoords(0, 359),
                                   new PgmCellCoords(0, 0),
                                   new PgmCellCoords(0, 1),
                                   new PgmCellCoords(0, 2),
                                   new PgmCellCoords(1, 359),
                                   new PgmCellCoords(1, 0),
                                   new PgmCellCoords(1, 1),
                                   new PgmCellCoords(1, 2),
                                   new PgmCellCoords(2, 359),
                                   new PgmCellCoords(2, 0),
                                   new PgmCellCoords(2, 1),
                                   new PgmCellCoords(2, 2),
                               },
                           },
                           new object[]
                           {
                               new PgmCellCoords(90, 0),
                               1,
                               1,
                               new List<PgmCellCoords>
                               {
                                   new PgmCellCoords(89, 359),
                                   new PgmCellCoords(89, 0),
                                   new PgmCellCoords(89, 1),
                                   new PgmCellCoords(89, 2),
                                   new PgmCellCoords(90, 359),
                                   new PgmCellCoords(90, 0),
                                   new PgmCellCoords(90, 1),
                                   new PgmCellCoords(90, 2),
                                   new PgmCellCoords(89, 179),
                                   new PgmCellCoords(89, 180),
                                   new PgmCellCoords(89, 181),
                                   new PgmCellCoords(89, 182),
                                   new PgmCellCoords(88, 179),
                                   new PgmCellCoords(88, 180),
                                   new PgmCellCoords(88, 181),
                                   new PgmCellCoords(88, 182),
                               },
                           },
                           new object[]
                           {
                               new PgmCellCoords(-90, 0),
                               1,
                               1,
                               new List<PgmCellCoords>
                               {
                                   new PgmCellCoords(-89, 179),
                                   new PgmCellCoords(-89, 180),
                                   new PgmCellCoords(-89, 181),
                                   new PgmCellCoords(-89, 182),
                                   new PgmCellCoords(-90, 359),
                                   new PgmCellCoords(-90, 0),
                                   new PgmCellCoords(-90, 1),
                                   new PgmCellCoords(-90, 2),
                                   new PgmCellCoords(-89, 359),
                                   new PgmCellCoords(-89, 0),
                                   new PgmCellCoords(-89, 1),
                                   new PgmCellCoords(-89, 2),
                                   new PgmCellCoords(-88, 359),
                                   new PgmCellCoords(-88, 0),
                                   new PgmCellCoords(-88, 1),
                                   new PgmCellCoords(-88, 2),
                               },
                           },
                           new object[]
                           {
                               new PgmCellCoords(-90, 359),
                               1,
                               1,
                               new List<PgmCellCoords>
                               {
                                   new PgmCellCoords(-89, 178),
                                   new PgmCellCoords(-89, 179),
                                   new PgmCellCoords(-89, 180),
                                   new PgmCellCoords(-89, 181),
                                   new PgmCellCoords(-90, 358),
                                   new PgmCellCoords(-90, 359),
                                   new PgmCellCoords(-90, 0),
                                   new PgmCellCoords(-90, 1),
                                   new PgmCellCoords(-89, 358),
                                   new PgmCellCoords(-89, 359),
                                   new PgmCellCoords(-89, 0),
                                   new PgmCellCoords(-89, 1),
                                   new PgmCellCoords(-88, 358),
                                   new PgmCellCoords(-88, 359),
                                   new PgmCellCoords(-88, 0),
                                   new PgmCellCoords(-88, 1),
                               },
                           },
                       };
            }
        }

        #endregion
    }
}
