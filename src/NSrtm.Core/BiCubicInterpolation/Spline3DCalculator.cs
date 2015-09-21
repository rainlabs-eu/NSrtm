using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NSrtm.Core.BiCubicInterpolation
{
    public class Spline3DCalculator
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
                double coefficient = x.Select((t, j) => _linearEquationCoefficients[i, j] * t)
                                      .Sum();
                coefficients.Add(coefficient);
            }
            return coefficients;
        }

        private static List<double> getParametersDescribingGrid(IReadOnlyList<IReadOnlyList<double>> values, double step)
        {
            var firstDerivativeX = values.Select(row => numericalDifferentation(row, step))
                                         .ToList()
                                         .AsReadOnly();

            var firstDerivativeY = values.Select((t, i) => values.Select(element => element[i]))
                                         .Select(column => numericalDifferentation(column.ToList(), step))
                                         .ToList()
                                         .AsReadOnly();
            var crossDerivative = new List<List<double>>();
            for (int i = 0; i < firstDerivativeY[0].Count; i++)
            {
                var col = firstDerivativeY.Select(p => p[i]);
                crossDerivative.Add(numericalDifferentation(col.ToList(), step));
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

        private static readonly int[,] _linearEquationCoefficients =

        {
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {-3, 3, 0, 0, -2, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {2, -2, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, -3, 3, 0, 0, -2, -1, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 2, -2, 0, 0, 1, 1, 0, 0},
            {-3, 0, 3, 0, 0, 0, 0, 0, -2, 0, -1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, -3, 0, 3, 0, 0, 0, 0, 0, -2, 0, -1, 0},
            {9, -9, -9, 9, 6, 3, -6, -3, 6, -6, 3, -3, 4, 2, 2, 1},
            {-6, 6, 6, -6, -3, -3, 3, 3, -4, 4, -2, 2, -2, -2, -1, -1},
            {2, 0, -2, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 2, 0, -2, 0, 0, 0, 0, 0, 1, 0, 1, 0},
            {-6, 6, 6, -6, -4, -2, 4, 2, -3, 3, -3, 3, -2, -1, -2, -1},
            {4, -4, -4, 4, 2, 2, -2, -2, 2, -2, 2, -2, 1, 1, 1, 1}
        };

        /// <summary>
        ///     This subroutine builds bicubic func spline.
        ///     For more information see https://en.wikipedia.org/wiki/Bicubic_interpolation
        /// </summary>
        /// <param name="values">function values,  (4×4)</param>
        /// <param name="step">Distance between the nodes</param>
        /// <returns>Func with spline interpolant</returns>
        public static BivariatePolynomial GetBiCubicSpline([NotNull] IReadOnlyList<IReadOnlyList<double>> values, double step)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (step <= 0) throw new ArgumentOutOfRangeException("step", "Step must be positive");
            if (values.Count != 4 || values.Any(value => value.Count != 4))
            {
                throw new ArgumentException("values", "Bicubic interpolation considers 16 elements(4×4)");
            }

            var x = getParametersDescribingGrid(values, step);

            var coefficients = getCoefficients(x);

            return new BivariatePolynomial(coefficients);
        }

        #endregion
    }
}
