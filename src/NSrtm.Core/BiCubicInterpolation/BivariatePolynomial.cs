using System;
using System.Collections.Generic;
using System.Linq;

namespace NSrtm.Core.BiCubicInterpolation
{
    public struct BivariatePolynomial
    {
        private readonly double[][] _coefficients;

        public BivariatePolynomial(IReadOnlyList<double> coefficients)
        {
            if (coefficients.Count != 16)
            {
                throw new ArgumentException("coefficients", "Bivariate polynomial use 16 coefficients.");
            }
            _coefficients = coefficients.Select((c, i) => new {Index = i, value = c})
                                        .GroupBy(p => p.Index / 4)
                                        .Select(c => c.Select(v => v.value)
                                                      .ToArray())
                                        .ToArray();
        }

        public double Evaluate(double x, double y)
        {
            return _coefficients.Select(coeff => coeff.UseHornerScheme(y))
                                .ToList()
                                .UseHornerScheme(x);
        }

        public double[][] Coefficients { get { return _coefficients; } }
    }

    internal static class HornersSchame
    {
        public static double UseHornerScheme(this IEnumerable<double> coefficients, double variable)
        {
            return coefficients.Reverse()
                               .Aggregate(
                                          (accumulator, coefficient) => accumulator * variable + coefficient);
        }
    }
}
