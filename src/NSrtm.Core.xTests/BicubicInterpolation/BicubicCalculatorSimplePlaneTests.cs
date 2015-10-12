using System.Collections.Generic;
using System.Linq;
using NSrtm.Core.BicubicInterpolation;
using Xunit;

namespace NSrtm.Core.xTests.BicubicInterpolation
{
    public class BicubicCalculatorSimplePlaneTests
    {
        [Theory]
        [MemberData("SimplePlane")]
        public void BicubicSplineCalculatorReturnsSimilarResultForNodes(
            List<List<double>> values,
            double step)
        {
            var spline = BicubicCalculator.GetSpline(values, step);
            var nodes = Enumerable.Range(0, 2);
            var positions = nodes.SelectMany(x => nodes.Select(y => new {x, y}))
                                 .ToList();
            foreach (var position in positions)
            {
                var result = spline.Evaluate(position.x, position.y);
                var expected = values[position.x + 1][position.y + 1];
                AssertDeep.Equal(result, expected, config => config.DoublePrecision = 1e-5);
            }
        }

        [Theory]
        [MemberData("SimplePlane")]
        public void BicubicSplineCalculatorReturnsCorrectResultsForSpaceBetweenNodes(
            List<List<double>> values,
            double step)
        {
            var spline = BicubicCalculator.GetSpline(values, step);
            var nodes = Enumerable.Range(0, 11).Select(p=>p*0.1);
            var positions = nodes.SelectMany(x => nodes.Select(y => new {x, y}))
                                 .ToList();
            var expected = values[0][0];
            foreach (var position in positions)
            {
                var result = spline.Evaluate(position.x, position.y);
                AssertDeep.Equal(result, expected, config => config.DoublePrecision = 1e-5);
            }
        }

        #region Static Members

        public static IReadOnlyList<object[]> SimplePlane
        {
            get
            {
                return new List<object[]>
                       {
                           new object[]
                           {
                               new List<List<double>>
                               {
                                   new List<double> {10, 10, 10, 10},
                                   new List<double> {10, 10, 10, 10},
                                   new List<double> {10, 10, 10, 10},
                                   new List<double> {10, 10, 10, 10},
                               },
                               1
                           },
                           new object[]
                           {
                               new List<List<double>>
                               {
                                   new List<double> {-10, -10, -10, -10},
                                   new List<double> {-10, -10, -10, -10},
                                   new List<double> {-10, -10, -10, -10},
                                   new List<double> {-10, -10, -10, -10},
                               },
                               1
                           }
                       };
            }
        }

        #endregion
    }
}
