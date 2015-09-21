using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NSrtm.Core.BicubicInterpolation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "This value type would never be compared")]
    public struct BivariatePolynomial
    {
        private readonly double[][] _coefficients;

        public BivariatePolynomial([NotNull] IReadOnlyList<double> coefficients)
        {
            if (coefficients == null) throw new ArgumentNullException("coefficients");
            if (coefficients.Count != 16) throw new ArgumentException("Bivariate polynomial use 16 coefficients.", "coefficients");
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
