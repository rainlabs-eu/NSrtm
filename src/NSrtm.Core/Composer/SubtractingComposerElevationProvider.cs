using System;
using System.Threading.Tasks;

namespace NSrtm.Core
{
    public class SubtractingComposerElevationProvider : IElevationProvider
    {
        private readonly IElevationProvider _providerPositive;
        private readonly IElevationProvider _providerNegative;

        /// <summary>
        /// Supports composing two elevations providers, one with positive sign and one with negative.
        /// 
        /// Common use case is using C over A and B over A providers composed into C over B
        /// 
        /// </summary>
        /// <param name="providerPositive">Provider with + sign (C over A)</param>
        /// <param name="providerNegative">Provider with - sign (B over A)</param>
        public SubtractingComposerElevationProvider(IElevationProvider providerPositive, IElevationProvider providerNegative)
        {
            if (providerPositive.ElevationBase != providerNegative.ElevationBase)
                throw new ArgumentException(string.Format("Base must be common for both providers, called with:" +
                                                          "providerPositive.ElevationBase == {0} and " +
                                                          "providerNegative.ElevationBase == {1}",
                                                          providerPositive.ElevationBase,
                                                          providerNegative.ElevationBase));

            _providerPositive = providerPositive;
            _providerNegative = providerNegative;
        }

        public double GetElevation(double latitude, double longitude)
        {
            double positive = _providerPositive.GetElevation(latitude, longitude);
            double negative = _providerNegative.GetElevation(latitude, longitude);
            return positive - negative;
        }

        public async Task<double> GetElevationAsync(double latitude, double longitude)
        {
            var positiveTask = _providerPositive.GetElevationAsync(latitude, longitude);
            var negativeTask = _providerNegative.GetElevationAsync(latitude, longitude);

            await Task.WhenAll(positiveTask, negativeTask);

            double positive = await positiveTask;
            double negative = await negativeTask;

            return positive - negative;
        }

        public Level ElevationBase { get { return _providerNegative.ElevationTarget; } }

        public Level ElevationTarget { get { return _providerPositive.ElevationTarget; } }
    }
}