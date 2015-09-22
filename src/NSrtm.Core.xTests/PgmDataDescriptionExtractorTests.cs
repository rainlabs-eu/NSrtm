using System;
using System.Collections.Generic;
using NSrtm.Core.Pgm;
using NSrtm.Core.Pgm.DataDesciption;
using Xunit;

namespace NSrtm.Core.xTests.Pgm
{
    public class PgmDataDescriptionExtractorTests
    {
        [Fact]
        public void MissingMagicNumberInPgmFileThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PgmDataDescriptionExtractor.getConstatantsFromPreamble(PgmWrongPreambles.WithoutMagicNumber));
        }

        [Fact]
        public void WrongMagicNumberInPgmFileThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PgmDataDescriptionExtractor.getConstatantsFromPreamble(PgmWrongPreambles.WithWrongMagicNumber));
        }

        [Fact]
        public void MissingFieldsInPgmFileThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PgmDataDescriptionExtractor.getConstatantsFromPreamble(PgmWrongPreambles.WithMissingFields));
        }

        [Theory]
        [MemberData("PgmGeographicLibPreamblesWithExpectedPgmDataDescription")]
        internal void PgmGeographicLibPreamblesAreProperlyConvertToDataDescriptions(string preamble, PgmDataDescription expectedDescription)
        {
            var actualDescription = PgmDataDescriptionExtractor.getConstatantsFromPreamble(preamble);
            AssertDeep.Equal(actualDescription, expectedDescription);
        }

        [Fact]
        public void CorrectPreambleWithChangedOffsetAndScaleFormatExtractCorrectly()
        {
            var expectedDescription = new PgmDataDescription(108, 5, 90, 0, 4320, 2161, 65535, 416, Level.GeoidEgm2008);
            var actualDescription = PgmDataDescriptionExtractor.getConstatantsFromPreamble(PgmAcceptablePreambles.WithChangedOffsetAndScaleFormat);
            AssertDeep.Equal(actualDescription, expectedDescription);
        }

        [Fact]
        public void CorrectPreambleWithChangedFieldsOrderExtractCorrectly()
        {
            var expectedDescription = PgmDataDescriptionExtractor.getConstatantsFromPreamble(PgmGeographicLibPreambles.Egm84WithStep300);
            var actualDescription = PgmDataDescriptionExtractor.getConstatantsFromPreamble(PgmAcceptablePreambles.WithChangedFieldsOrder);
            AssertDeep.Equal(actualDescription, expectedDescription);
        }

        #region Static Members

        public static IReadOnlyList<object[]> PgmGeographicLibPreamblesWithExpectedPgmDataDescription
        {
            get
            {
                return new List<object[]>
                       {
                           new object[] {PgmGeographicLibPreambles.Egm2008WithStep10, new PgmDataDescription(-108, 0.003, 90, 0, 21600, 10801, 65535, 418, Level.GeoidEgm2008)},
                           new object[] {PgmGeographicLibPreambles.Egm2008WithStep25, new PgmDataDescription(-108, 0.003, 90, 0, 8640, 4321, 65535, 418, Level.GeoidEgm2008) },
                           new object[] {PgmGeographicLibPreambles.Egm2008WithStep50, new PgmDataDescription(-108, 0.003, 90, 0, 4320, 2161, 65535, 418, Level.GeoidEgm2008) },
                           new object[] {PgmGeographicLibPreambles.Egm96WithStep50, new PgmDataDescription(-108, 0.003, 90, 0, 4320, 2161, 65535, 422, Level.GeoidEgm96) },
                           new object[] {PgmGeographicLibPreambles.Egm96WithStep150, new PgmDataDescription(-108, 0.003, 90, 0, 1440, 721, 65535, 422, Level.GeoidEgm96) },
                           new object[] {PgmGeographicLibPreambles.Egm84WithStep150, new PgmDataDescription(-108, 0.003, 90, 0, 1440, 721, 65535, 430, Level.GeoidEgm84) },
                           new object[] {PgmGeographicLibPreambles.Egm84WithStep300, new PgmDataDescription(-108, 0.003, 90, 0, 720, 361, 65535, 430, Level.GeoidEgm84)},
                       };
            }
        }

        public static IReadOnlyList<object[]> PgmAcceptablePreamblesWithExpectedPgmDataDescription
        {
            get
            {
                return new List<object[]>
                       {
                           new object[] {PgmAcceptablePreambles.WithChangedFieldsOrder, new PgmDataDescription(-108, 0.003, 90, 0, 21600, 10801, 65535, 403, Level.GeoidEgm84)},
                           new object[]
                           {
                               PgmAcceptablePreambles.WithChangedOffsetAndScaleFormat,
                               new PgmDataDescription(-108, 0.003, 90, 0, 8640, 4321, 65535, 403, Level.GeoidEgm2008)
                           },
                       };
            }
        }

        #endregion
    }
}
