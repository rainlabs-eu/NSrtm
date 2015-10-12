﻿using System;
using System.Collections.Generic;
using NSrtm.Core.BicubicInterpolation;
using Xunit;

namespace NSrtm.Core.xTests.BicubicInterpolation
{
    public class BicubicCalculatorWrongArgumentsTests
    {
        [Fact]
        public void EmptyValuesContainerThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => BicubicCalculator.GetSpline(null, 1));
        }

        [Fact]
        public void ValuesWithWrongSizeDimensionThrowsArgumentException()
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
        public void ZeroStepThrowsArgumentOutOfRangeException()
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
        public void NegativeStepThrowsArgumentOutOfRangeException()
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