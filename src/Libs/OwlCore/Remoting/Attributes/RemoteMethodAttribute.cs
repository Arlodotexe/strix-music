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
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RemoteMethodAttribute : Attribute, IMethodInterceptor
    {
        /// <inheritdoc/>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            var trace = new StackTrace(true);
            var frames = trace.GetFrames();

            for (int i = 0; i < frames.Length; i++)
            {
                StackFrame? frame = frames[i];
                if (frame is null)
                    continue;

                // Find the caller method name
                var frameMethod = frame.GetMethod();
                if (frameMethod.Name != methodbase.Name)
                    continue;

                // Check if the invoker was the OwlCore.Remoting lib between 1-5 frames back
                for (; i <= i + 8; i++)
                {
                    if (frames.Length == i)
                        break;

                    if (frames[i].GetMethod().Name == nameof(MemberRemote.MessageHandler_DataReceived))
                        return;
                }

                break;
            }

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
