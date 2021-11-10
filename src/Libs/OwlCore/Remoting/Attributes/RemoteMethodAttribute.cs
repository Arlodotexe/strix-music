using System;
using System.Reflection;
using System.Threading;
using Cauldron.Interception;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Mark a method or class with this attribute to opt into remote method invocation via <see cref="MemberRemote"/>.
    /// </summary>
    /// <remarks>
    /// For IL weaving to take effect, you must install and add a reference to <see href="https://www.nuget.org/packages/Cauldron.BasicInterceptors/3.2.3">Cauldron.BasicInterceptors</see> directly in the project that uses this attribute.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RemoteMethodAttribute : Attribute, IMethodInterceptor
    {
        /// <inheritdoc/>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            if (MethodIsEventHandlerAdd(methodbase) || MethodIsEventHandlerRemove(methodbase))
                return;

            // Check if the invoker was the OwlCore.Remoting library.
            // Don't re-emit "entered" to the library if so.
            lock (MemberRemote.MemberHandleExpectancyMap)
            {
                if (MemberRemote.MemberHandleExpectancyMap.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var expectedInstance) && ReferenceEquals(expectedInstance, instance))
                {
                    MemberRemote.MemberHandleExpectancyMap.Remove(Thread.CurrentThread.ManagedThreadId);
                    return;
                }
            }

            var args = new MethodEnteredEventArgs(declaringType, instance, methodbase, values);
            Entered?.Invoke(this, args);
        }

        /// <inheritdoc/>
        public bool OnException(Exception e)
        {
            ExceptionRaised?.Invoke(this, e);
            return true;
        }

        /// <inheritdoc/>
        public void OnExit() => Exited?.Invoke(this, System.EventArgs.Empty);

        /// <summary>
        /// Raised when the attached method is fired.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IMethodInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler<MethodEnteredEventArgs>? Entered;

        /// <summary>
        /// Raised when the event is finished executing and is about to exit.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IMethodInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler? Exited;

        /// <summary>
        /// Raised when an exception occurs in the method.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IMethodInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler<Exception>? ExceptionRaised;

        private bool MethodIsEventHandlerAdd(MethodBase methodbase)
        {
            var events = methodbase.DeclaringType.GetEvents();

            foreach (var ev in events)
                if (ev.GetAddMethod() == methodbase)
                    return true;

            return false;
        }

        private bool MethodIsEventHandlerRemove(MethodBase methodbase)
        {
            var events = methodbase.DeclaringType.GetEvents();

            foreach (var ev in events)
                if (ev.GetRemoveMethod() == methodbase)
                    return true;

            return false;
        }
    }
}
