using System;
using System.Diagnostics;
using System.Reflection;
using Cauldron.Interception;
using OwlCore.Remoting.EventArgs;

namespace OwlCore.Remoting.Attributes
{

    /// <summary>
    /// Attribute used in conjuction with <see cref="MemberRemote"/>.
    /// Mark a method with this attribute to opt into remote method changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RemoteMethodAttribute : Attribute, IMethodInterceptor
    {
        /// <inheritdoc/>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            var trace = new StackTrace(true);
            var frames = trace.GetFrames();

            if (frames[3].GetMethod().Name == nameof(MethodBase.Invoke) &&
                frames[8].GetMethod().Name == nameof(MemberRemote.MessageHandler_DataReceived))
                return;

            Entered?.Invoke(this, new MethodEnteredEventArgs(declaringType, instance, methodbase, values));
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
    }
}
