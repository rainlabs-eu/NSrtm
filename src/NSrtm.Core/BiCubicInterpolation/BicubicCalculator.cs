﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NSrtm.Core.BicubicInterpolation
{
    public static class BicubicCalculator
    {
        /// <summary>
        ///     Finding derivatives from 2-d function values - https://en.wikipedia.org/wiki/Bicubic_interpolation
        ///     using centered differencing formula (5.4)
        ///     http://www2.math.umd.edu/~dlevy/classes/amsc466/lecture-notes/differentiation-chap.pdf
        /// </summary>
        private static List<double> numericalDifferentation(
            IReadOnlyList<double> values,
            double step)
        {
            var derivative = new List<double>();
            for (int i = 1; i < values.Count - 1; i++)
            {
                derivative.Add(centeredDifferencingFormula(values[i - 1], values[i + 1], step));
            }
            return derivative;
        }

        private static double centeredDifferencingFormula(double previous, double next, double step)
        {
            return (next - previous) / (2 * step);
        }

        private static List<double> getCoefficients(List<double> x)
        {
            var coefficients = new List<double>();

            for (int i = 0; i < _linearEquationCoefficients.GetLength(0); i++)
            {
                double coefficient = x.Select((t, j) => _linearEquationCoefficients[i][j] * t)
                                      .Sum();
                coefficients.Add(coefficient);
            }
            return coefficients;
        }

        private static List<double> getParametersDescribingGrid(IReadOnlyList<IReadOnlyList<double>> values)
        {
            const double pointsDistance = 1;

            var firstDerivativeX = values.Select(row => numericalDifferentation(row, step: pointsDistance))
                                         .ToList()
                                         .AsReadOnly();

            var firstDerivativeY = values.Select((t, i) => values.Select(element => element[i]))
                                         .Select(column => numericalDifferentation(column.ToList(), step: pointsDistance))
                                         .ToList()
                                         .AsReadOnly();
            var crossDerivative = new List<List<double>>();
            for (int i = 0; i < firstDerivativeY[0].Count; i++)
            {
                var col = firstDerivativeY.Select(p => p[i]);
                crossDerivative.Add(numericalDifferentation(col.ToList(), step: pointsDistance));
            }

            var x = new List<double>
                    {
                        values[1][1],
                        values[1][2],
                        values[2][1],
                        values[2][2],
                        firstDerivativeX[1][0],
                        firstDerivativeX[1][1],
                        firstDerivativeX[2][0],
                        firstDerivativeX[2][1],
                        firstDerivativeY[1][0],
                        firstDerivativeY[1][1],
                        firstDerivativeY[2][0],
                        firstDerivativeY[2][1],
                        crossDerivative[0][0],
                        crossDerivative[0][1],
                        crossDerivative[1][0],
                        crossDerivative[1][1]
                    };
            return x;
        }

        #region Static Members

        private static readonly int[][] _linearEquationCoefficients =
        {
            new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {-3, 3, 0, 0, -2, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {2, -2, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, -3, 3, 0, 0, -2, -1, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 2, -2, 0, 0, 1, 1, 0, 0},
            new int[] {-3, 0, 3, 0, 0, 0, 0, 0, -2, 0, -1, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, -3, 0, 3, 0, 0, 0, 0, 0, -2, 0, -1, 0},
            new int[] {9, -9, -9, 9, 6, 3, -6, -3, 6, -6, 3, -3, 4, 2, 2, 1},
            new int[] {-6, 6, 6, -6, -3, -3, 3, 3, -4, 4, -2, 2, -2, -2, -1, -1},
            new int[] {2, 0, -2, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 2, 0, -2, 0, 0, 0, 0, 0, 1, 0, 1, 0},
            new int[] {-6, 6, 6, -6, -4, -2, 4, 2, -3, 3, -3, 3, -2, -1, -2, -1},
            new int[] {4, -4, -4, 4, 2, 2, -2, -2, 2, -2, 2, -2, 1, 1, 1, 1}
        };

        /// <summary>
        ///     This subroutine builds bicubic func spline.
        ///     Normalized, interpolates range 0 to 1
        ///     For more information see https://en.wikipedia.org/wiki/Bicubic_interpolation
        /// </summary>
        /// <param name="values">function values,  (4×4)</param>
        /// <returns>Func with spline interpolant</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This way of passing function values brings less misunderstandings")]
        public static BivariatePolynomial GetSpline([NotNull] IReadOnlyList<IReadOnlyList<double>> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (values.Count != 4 || values.Any(value => value.Count != 4))
            {
                throw new ArgumentException("Bicubic interpolation considers 16 elements(4×4)", "values");
            }

            var x = getParametersDescribingGrid(values);

            var coefficients = getCoefficients(x);

            return new BivariatePolynomial(coefficients);
        }

        #endregion
    }
}
