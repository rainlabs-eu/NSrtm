using System;
using System.Collections.Generic;
using NSrtm.Core.BicubicInterpolation;
using Xunit;

namespace NSrtm.Core.xTests
{
    public class Spline3DCalculatorSimpleTests
    {
        [Fact]
        public void EmptyValuesContainerThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => BicubicCalculator.GetSpline(null, 1));
        }

        [Fact]
        public void ValuesWithWrongSizeDimensionThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentException>(
                                                 () =>
                                                 BicubicCalculator.GetSpline(
                                                                                     new List<List<double>>
                                                                                     {
                                                                                         new List<double> {1, 2, 3, 4},
                                                                                         new List<double> {3, 4, 5, 6},
                                                                                         new List<double> {1, 2, 3}
                                                                                     },
                                                                                     1));
        }

        [Fact]
        public void ZeroStepThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                                                 () =>
                                                 BicubicCalculator.GetSpline(
                                                                                     new List<List<double>>
                                                                                     {
                                                                                         new List<double> {1, 2, 3, 4},
                                                                                         new List<double> {3, 4, 5, 6},
                                                                                         new List<double> {1, 2, 3}
                                                                                     },
                                                                                     0));
        }

        [Fact]
        public void NegativeStepThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                                                 () =>
                                                 BicubicCalculator.GetSpline(
                                                                                     new List<List<double>>
                                                                                     {
                                                                                         new List<double> {1, 2, 3, 4},
                                                                                         new List<double> {3, 4, 5, 6},
                                                                                         new List<double> {1, 2, 3}
                                                                                     },
                                                                                     -1));
        }
    }
}
