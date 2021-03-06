﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace NSrtm.Core.BicubicInterpolation
{
    [SuppressMessage("Microsoft.Performance",
        "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes",
        Justification = "This value type would never be compared")]
    public struct BivariatePolynomial
    {
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Bicubic_interpolation alpha matrix
        ///     a33 a32 a31 a30
        ///     a23 a22 a21 a20
        ///     a13 a12 a11 a10
        ///     a03 a02 a01 a00
        /// </summary>
        private readonly double[][] _reversedCoefficients;

        public BivariatePolynomial([NotNull] IReadOnlyList<double> coefficients)
        {
            if (coefficients == null) throw new ArgumentNullException("coefficients");
            if (coefficients.Count != 16) throw new ArgumentException("Bivariate polynomial use 16 coefficients.", "coefficients");
            _reversedCoefficients = coefficients.Select((c, i) => new {Index = i, value = c})
                                                .GroupBy(p => p.Index / 4)
                                                .Select(c => c.Select(v => v.value)
                                                              .Reverse()
                                                              .ToArray())
                                                .Reverse()
                                                .ToArray();
        }

        public double Evaluate(double x, double y)
        {
            var hornersResultsTermsY = new double[4];
            for (int i = 0; i < 4; i++)
            {
                hornersResultsTermsY[i] = _reversedCoefficients[i].UseHornerScheme(y);
            }
            var hornerResultTermsX = hornersResultsTermsY.UseHornerScheme(x);
            return hornerResultTermsX;
        }
    }

    internal static class HornersSchame
    {
        public static double UseHornerScheme(this IEnumerable<double> coefficients, double variable)
        {
            double result = 0;
            foreach (var coefficient in coefficients)
            {
                result = result * variable + coefficient;
            }
            return result;
        }
    }
}
