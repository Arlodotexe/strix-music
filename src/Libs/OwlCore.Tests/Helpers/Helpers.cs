using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace OwlCore.Tests
{
    internal static class Helpers
    {
        public static bool SmartEquals(object? originalValue, Type? originalType, object? deserValue, Type? deserType)
        {
            if (originalValue is IEnumerable)
            {
                var originalEnumerable = originalValue as IEnumerable;
                var deserEnumerable = deserValue as IEnumerable;

                Assert.IsNotNull(originalEnumerable);
                Assert.IsNotNull(deserEnumerable);

                // Only foreach can be used, so we need to track index manually to check inner values.
                var originalIndex = -1;
                foreach (var originalItem in originalEnumerable)
                {
                    originalIndex++;

                    var deserIndex = -1;
                    foreach (var deserItem in deserEnumerable)
                    {
                        deserIndex++;

                        if (deserIndex == originalIndex)
                        {
                            if (!SmartEquals(originalItem, originalItem.GetType(), deserItem, deserItem.GetType()))
                                return false;
                        }
                    }
                }

                return true;
            }
            else if (originalValue != null && !originalType.IsPrimitive && !originalType.IsValueType)
            {
                return CrawlAndCheckObjectProperties(originalValue, originalType, deserValue, deserType);
            }
            else
            {
                return Equals(originalValue, deserValue);
            }
        }

        public static void SmartAssertEqual(object? originalValue, Type? originalType, object? deserValue, Type? deserType)
        {
            if (originalValue is IEnumerable)
            {
                var originalEnumerable = originalValue as IEnumerable;
                var deserEnumerable = deserValue as IEnumerable;

                Assert.IsNotNull(originalEnumerable);
                Assert.IsNotNull(deserEnumerable);

                // Only foreach can be used, so we need to track index manually to check inner values.
                var originalIndex = -1;
                foreach (var originalItem in originalEnumerable)
                {
                    originalIndex++;

                    var deserIndex = -1;
                    foreach (var deserItem in deserEnumerable)
                    {
                        deserIndex++;

                        if (deserIndex == originalIndex)
                        {
                            SmartAssertEqual(originalItem, originalItem?.GetType(), deserItem, deserItem?.GetType());
                        }
                    }
                }
            }
            else if (originalValue != null && !originalType.IsPrimitive && !originalType.IsValueType)
            {
                CrawlAndAssertObjectProperties(originalValue, originalType, deserValue, deserType);
            }
            else
            {
                Assert.AreEqual(originalValue, deserValue);
            }
        }

        private static void CrawlAndAssertObjectProperties(object? originalValue, Type? originalType, object? deserValue, Type? deserType)
        {
            Assert.AreEqual(originalType, deserType);

            var originalParamProps = originalType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var deserParamProps = deserType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            if (originalParamProps is null && !(deserParamProps is null))
                Assert.Fail();

            if (!(originalParamProps is null) && deserParamProps is null)
                Assert.Fail();

            if (originalParamProps is null && deserParamProps is null)
                return;

            Assert.IsNotNull(originalParamProps);
            Assert.IsNotNull(deserParamProps);

            for (int o = 0; o < originalParamProps.Length; o++)
            {
                var originalParamProp = originalParamProps[o];
                var deserParamProp = deserParamProps[o];

                var originalParamValue = originalParamProp.GetValue(originalValue);
                var deserParamValue = deserParamProp.GetValue(deserValue);

                SmartAssertEqual(originalParamValue, originalParamProp.PropertyType, deserParamValue, deserParamProp.PropertyType);
            }
        }

        private static bool CrawlAndCheckObjectProperties(object? originalValue, Type? originalType, object? deserValue, Type? deserType)
        {
            var originalParamProps = originalType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var deserParamProps = deserType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            if (originalParamProps is null && !(deserParamProps is null))
                return false;

            if (!(originalParamProps is null) && deserParamProps is null)
                return false;

            if (originalParamProps is null && deserParamProps is null)
                return true;

            Assert.IsNotNull(originalParamProps);
            Assert.IsNotNull(deserParamProps);

            for (int o = 0; o < originalParamProps.Length; o++)
            {
                var originalParamProp = originalParamProps[o];
                var deserParamProp = deserParamProps[o];

                var originalParamValue = originalParamProp.GetValue(originalValue);
                var deserParamValue = deserParamProp.GetValue(deserValue);

                if (!SmartEquals(originalParamValue, originalParamProp.PropertyType, deserParamValue, deserParamProp.PropertyType))
                    return false;
            }

            return true;
        }
    }
}
