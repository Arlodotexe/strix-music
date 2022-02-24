using System;

namespace StrixMusic.Sdk.Tests
{
    /// <summary>
    /// When thrown, indicates that the given type was accessed.
    /// </summary>
    /// <typeparam name="T">The type that was accessed.</typeparam>
    internal class AccessedException<T> : Exception
    {
        public Type Type { get; } = typeof(T);
    }
}
