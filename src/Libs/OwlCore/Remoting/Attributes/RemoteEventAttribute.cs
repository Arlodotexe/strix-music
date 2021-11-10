using System;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Mark a method or class with this attribute to opt into remote event raising via <see cref="MemberRemote"/>.
    /// </summary>
    /// <remarks>
    /// This is prototype code and should not be used.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Class)]
    internal class RemoteEventAttribute : Attribute
    {
        public static void HandleEventInvocation(params object[] parameters)
        {
            EventFired?.Invoke(null, parameters);
        }

        public static event EventHandler<object[]>? EventFired;
    }
}
