using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OwlCore.Extensions;

namespace StrixMusic.Sdk.Tests
{
    internal static class Helpers
    {
        public static void AssertAllThrowsOnMemberAccess<TException>(object value, bool mustContainMembers = true, Func<MemberInfo, bool>? customFilter = null, params Type[] typesToExclude)
            where TException : Exception
        {
            AssertAllThrowsOnMemberAccess(value.GetType(), value, mustContainMembers, customFilter, typesToExclude, new[] { typeof(TException) });
        }


        public static void AssertAllThrowsOnMemberAccess<TException, TInterfaceFilter>(object value, bool mustContainMembers = true, Func<MemberInfo, bool>? customFilter = null, params Type[] typesToExclude)
            where TException : Exception
        {
            AssertAllThrowsOnMemberAccess(typeof(TInterfaceFilter), value, mustContainMembers, customFilter, typesToExclude, new[] { typeof(TException) });
        }

        public static void AssertAllThrowsOnMemberAccess<TInterfaceFilter>(object value, Type[] expectedExceptions, bool mustContainMembers = true, Func<MemberInfo, bool>? customFilter = null, Type[]? typesToExclude = null)
        {
            AssertAllThrowsOnMemberAccess(typeof(TInterfaceFilter), value, mustContainMembers, customFilter, typesToExclude, expectedExceptions);
        }

        public static void AssertAllThrowsOnMemberAccess(Type type, object value, bool mustContainMembers = true, Func<MemberInfo, bool>? customFilter = null, Type[]? typesToExclude = null, Type[]? expectedExceptions = null)
        {
            Assert.IsNotNull(expectedExceptions);
            var thrownExceptions = new List<Type>();

            var instanceType = value.GetType();
            var typeMethods = type.GetMethods();
            var allExcludedMethods = typesToExclude?.SelectMany(x => x.GetMethods()) ?? Enumerable.Empty<MethodInfo>();

            var methods = instanceType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                      .Where(x => customFilter?.Invoke(x) ?? true)
                                      .Where(x => typeMethods.Any(y => x.ToString() == y.ToString()))
                                      .Where(x => !allExcludedMethods.Any() || !allExcludedMethods.Any(y => y.ToString() == x.ToString()))
                                      .ToArray();

            if (instanceType != type)
            {
                methods = instanceType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                      .Where(x => customFilter?.Invoke(x) ?? true)
                                      .Where(x => typeMethods.Any(y => x.ToString() == y.ToString()))
                                      .Where(x => !allExcludedMethods.Any() || !allExcludedMethods.Any(y => y.ToString() == x.ToString()))
                                      .ToArray();
            }

            if (mustContainMembers)
            {
                Assert.AreNotEqual(0, methods.Length);
            }

            foreach (var method in methods)
            {
                try
                {
                    method!.Invoke(value, method.GetParameters().Select(TransformMethodParam).ToArray());

                    object? TransformMethodParam(ParameterInfo param)
                    {
                        if (param.DefaultValue is DBNull)
                            return null;

                        if (param.ParameterType.IsValueType)
                            return Activator.CreateInstance(param.ParameterType);

                        return param.RawDefaultValue;
                    }
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException is AggregateException aggregateException)
                    {
                        foreach (var innerEx in aggregateException.InnerExceptions)
                        {
                            thrownExceptions.Add(innerEx.GetType());
                            Assert.IsTrue(expectedExceptions.Contains(innerEx!.GetType()), $"{method.Name} did not throw the correct exception.");
                        }
                    }
                    else
                    {
                        thrownExceptions.Add(ex.InnerException!.GetType());
                        Assert.IsTrue(expectedExceptions.Contains(ex.InnerException!.GetType()), $"{method.Name} did not throw the correct exception.");
                    }
                }
            }

            foreach (var item in expectedExceptions)
                Assert.IsTrue(thrownExceptions.Contains(item));
        }

        public static bool SmartEquals(object? originalValue, object? deserValue, bool recursive = true)
        {
            return SmartEquals(originalValue, originalValue?.GetType(), deserValue, deserValue?.GetType(), recursive);
        }

        public static bool SmartEquals(object? originalValue, Type? originalType, object? deserValue, Type? deserType, bool recursive = true, List<object>? crawledObjects = null)
        {
            crawledObjects ??= new List<object>();

            if (originalValue is IEnumerable)
            {
                if (crawledObjects.Contains(originalValue))
                    return true;
                else
                    crawledObjects.Add(originalValue);

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
                            if (!SmartEquals(originalItem, originalItem.GetType(), deserItem, deserItem.GetType(), recursive, crawledObjects))
                                return false;
                        }
                    }
                }

                return true;
            }
            else if (originalValue != null && !(originalType?.IsPrimitive ?? true) && !(originalType?.IsValueType ?? true))
            {
                if (crawledObjects.Contains(originalValue))
                    return true;
                else
                    crawledObjects.Add(originalValue);

                return CrawlAndCheckObjectProperties(originalValue, originalType, deserValue, deserType, recursive, crawledObjects);
            }
            else
            {
                return Equals(originalValue, deserValue);
            }
        }

        public static void SmartAssertEqual(object? originalValue, object? deserValue, bool recursive = true)
        {
            SmartAssertEqual(originalValue, originalValue?.GetType(), deserValue, deserValue?.GetType(), recursive);
        }

        public static void SmartAssertEqual(object? originalValue, Type? originalType, object? deserValue, Type? deserType, bool recursive = true, List<object>? crawledObjects = null)
        {
            crawledObjects ??= new List<object>();

            if (originalValue is IEnumerable)
            {
                if (crawledObjects.Contains(originalValue))
                    return;
                else
                    crawledObjects.Add(originalValue);

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
                            SmartAssertEqual(originalItem, originalItem?.GetType(), deserItem, deserItem?.GetType(), recursive, crawledObjects);
                        }
                    }
                }
            }
            else if (originalValue != null && !(originalType?.IsPrimitive ?? true) && !(originalType?.IsValueType ?? true))
            {
                if (crawledObjects.Contains(originalValue))
                    return;
                else
                    crawledObjects.Add(originalValue);

                CrawlAndAssertObjectProperties(originalValue, originalType, deserValue, deserType, recursive, crawledObjects);
            }
            else
            {
                Assert.AreEqual(originalValue, deserValue);
            }
        }

        private static void CrawlAndAssertObjectProperties(object? originalValue, Type? originalType, object? deserValue, Type? deserType, bool recursive = true, List<object>? crawledObjects = null)
        {
            var originalParamProps = originalType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var deserParamProps = deserType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            // Check null props
            if (originalParamProps is null && deserParamProps is not null)
                Assert.Fail($"{nameof(originalParamProps)} was unexpectedly null.");

            if (originalParamProps is not null && deserParamProps is null)
                Assert.Fail($"{nameof(deserParamProps)} was unexpectedly null.");

            if (originalParamProps is null && deserParamProps is null)
                return;

            // Check null values
            if (originalValue is null && deserValue is not null)
                Assert.Fail($"{nameof(deserValue)} {deserValue} was not null, but {nameof(originalValue)} was null.");

            if (originalValue is not null && deserValue is null)
                Assert.Fail($"{nameof(originalValue)} {originalValue} was not null, but {nameof(deserValue)} was null.");

            if (originalValue is null && deserValue is null)
                return;

            Assert.IsNotNull(originalParamProps);
            Assert.IsNotNull(deserParamProps);
            Assert.AreNotEqual(0, originalParamProps.Length);

            for (int o = 0; o < originalParamProps.Length; o++)
            {
                try
                {
                    var originalParamProp = originalParamProps[o];
                    var deserParamProp = deserParamProps.First(x => x.Name == originalParamProp.Name);

                    var originalParamValue = originalParamProp.GetValue(originalValue);
                    var deserParamValue = deserParamProp.GetValue(deserValue);

                    // If this is an object we can crawl recursively.
                    if (!recursive && originalValue is not IEnumerable && (originalValue != null && !(originalType?.IsPrimitive ?? true) && !(originalType?.IsValueType ?? true)))
                        return;

                    SmartAssertEqual(originalParamValue, originalParamProp.PropertyType, deserParamValue, deserParamProp.PropertyType, recursive, crawledObjects);
                }
                catch (NotImplementedException)
                {
                }
                catch (TargetInvocationException ex) when (ex.InnerException is NotImplementedException)
                {
                }
            }
        }

        private static bool CrawlAndCheckObjectProperties(object? originalValue, Type? originalType, object? deserValue, Type? deserType, bool recursive = true, List<object>? crawledObjects = null)
        {
            var originalParamProps = originalType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var deserParamProps = deserType?.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            // Check null props
            if (originalParamProps is null && deserParamProps is not null)
                return false;

            if (originalParamProps is not null && deserParamProps is null)
                return false;

            if (originalParamProps is null && deserParamProps is null)
                return true;

            // Check null values
            if (originalValue is null && deserValue is not null)
                return false;

            if (originalValue is not null && deserValue is null)
                return false;

            if (originalValue is null && deserValue is null)
                return true;

            Assert.IsNotNull(originalParamProps);
            Assert.IsNotNull(deserParamProps);
            Assert.AreNotEqual(0, originalParamProps.Length);

            for (int o = 0; o < originalParamProps.Length; o++)
            {
                var originalParamProp = originalParamProps[o];
                var deserParamProp = deserParamProps.First(x => x.Name == originalParamProp.Name);

                try
                {
                    var originalParamValue = originalParamProp.GetValue(originalValue);
                    var deserParamValue = deserValue is not null ? deserParamProp.GetValue(deserValue) : null;

                    // If this is an object we can crawl recursively.
                    if (!recursive && originalValue is not IEnumerable && (originalValue != null && !(originalType?.IsPrimitive ?? true) && !(originalType?.IsValueType ?? true)))
                        return true;

                    if (!SmartEquals(originalParamValue, originalParamProp.PropertyType, deserParamValue, deserParamProp.PropertyType, recursive, crawledObjects))
                        return false;
                }
                catch (TargetInvocationException ex)
                {
                    // ignore the properties that aren't implemented
                    if (ex.InnerException is not NotImplementedException)
                        throw ex;
                }
            }

            return true;
        }
    }
}
