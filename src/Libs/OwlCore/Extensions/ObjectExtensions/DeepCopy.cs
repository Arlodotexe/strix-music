using System;
using System.Collections.Generic;
using System.Reflection;
using OwlCore.EqualityComparers;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="object"/>.
    /// </summary>
    public static partial class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Check if a type is primitive.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is primitive.</returns>
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        /// <summary>
        /// Deep copy an object.
        /// </summary>
        /// <param name="originalObject">The original object to copy.</param>
        /// <remarks>
        /// Source: <see href="https://github.com/Burtsev-Alexey/net-object-deep-copy"/>
        /// </remarks>
        public static object DeepCopy(this object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect))
                return originalObject;

            if (visited.ContainsKey(originalObject))
                return visited[originalObject];

            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
                throw new InvalidOperationException($"Cannot copy a {nameof(Delegate)}");

            var cloneObject = CloneMethod.Invoke(originalObject, null);

            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.Traverse((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }

            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);

            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType == null)
                return;

            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
            CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool>? filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false)
                    continue;

                if (IsPrimitive(fieldInfo.FieldType)) 
                    continue;

                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        /// <summary>
        /// Deep copy an object.
        /// </summary>
        /// <typeparam name="T">The type to copy and return.</typeparam>
        /// <param name="original">The original object to copy.</param>
        /// <returns>A deep copied object.</returns>
        public static T DeepCopy<T>(this T original) where T : class
        {
            return (T)DeepCopy((object)original);
        }
    }
}