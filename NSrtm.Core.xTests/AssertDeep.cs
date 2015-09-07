using System;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace NSrtm.Core.xTests
{
    public static class AssertDeep
    {
        public static void Equal<T>(T obj1, T obj2, params Action<ComparisonConfig>[] config)
        {
            var compareConfig = new ComparisonConfig();
            foreach (var action in config)
            {
                action(compareConfig);
            }
            var compareLogic = new CompareLogic(compareConfig);

            var result = compareLogic.Compare(obj1, obj2);
            Assert.True(result.AreEqual, result.DifferencesString);
        }

        public static void Equal<T>(this CompareLogic @this, T obj1, T obj2)
        {
            var result = @this.Compare(obj1, obj2);
            Assert.True(result.AreEqual, result.DifferencesString);
        }
    }
}
