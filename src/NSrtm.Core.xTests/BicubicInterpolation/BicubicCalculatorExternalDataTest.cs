using System.Collections.Generic;
using NSrtm.Core.BicubicInterpolation;
using Xunit;

namespace NSrtm.Core.xTests.BicubicInterpolation
{
    public class BicubicCalculatorExternalDataTest
    {
        [Theory]
        [MemberData("ExternalData")]
        public void BicubicSplineCalculatorReturnSimilarResultToExternalData(
            List<List<double>> values,
            double step,
            double xPosition,
            double yPosition,
            double expectedResult)
        {
            var spline = BicubicCalculator.GetSpline(values, step);
            var result = spline.Evaluate( xPosition, yPosition);
            AssertDeep.Equal(result, expectedResult, config => config.DoublePrecision = 1e-1);
        }

        #region Static Members

        public static IReadOnlyList<object[]> ExternalData
        {
            get
            {
                return new List<object[]>
                       {
                           new object[]
                           {
                               new List<List<double>>
                               {
                                   new List<double> {33, 33, 33, 33},
                                   new List<double> {31, 30, 31, 32},
                                   new List<double> {28, 29, 29, 30},
                                   new List<double> {26, 27, 28, 29},
                               },
                               1,
                               0.55,
                               0.7,
                               29.5
                           }
                       };
            }
        }

        #endregion
    }
}
