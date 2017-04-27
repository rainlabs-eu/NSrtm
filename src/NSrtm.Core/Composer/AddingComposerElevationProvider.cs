using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NSrtm.Core
{
    public class AddingComposerElevationProvider : IElevationProvider
    {
        private readonly IElevationProvider[] _providers;

        /// <summary>
        ///     Elevation provider that sums elevations from other providers
        ///     Common use case is to combine C over B provider with B over A provider to create C over A
        ///     The validity of data is not checked
        /// </summary>
        /// <param name="providers">Composite providers in correct order e.g D over C, C over B, B over A</param>
        public AddingComposerElevationProvider([NotNull] params IElevationProvider[] providers)
        {
            if (providers == null)
                throw new ArgumentNullException(nameof(providers));
            if (providers.Any(p => p == null))
                throw new ArgumentNullException(nameof(providers), "At least one provider is null");
            var correctlyChained =
                providers.Zip(providers.Skip(1), (p1, p2) => p1.ElevationBase == p2.ElevationTarget).All(ok => ok);
            if (!correctlyChained)
                throw new ArgumentException(
                    "Composite providers targets and bases don't match (are not chained correctly)", nameof(providers));

            _providers = providers;
        }

        public double GetElevation(double latitude, double longitude)
        {
            return _providers
                .Select(p => p.GetElevation(latitude, longitude))
                .Sum();
        }

        public async Task<double> GetElevationAsync(double latitude, double longitude)
        {
            var tasks = _providers.Select(p => p.GetElevationAsync(latitude, longitude));
            var results = await Task.WhenAll(tasks);
            return results.Sum();
        }

        public Level ElevationBase => _providers.Last().ElevationBase;

        public Level ElevationTarget => _providers.First().ElevationTarget;
    }
}