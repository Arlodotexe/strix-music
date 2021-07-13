using System;
using System.Diagnostics;
using System.Reflection;
using Cauldron.Interception;
using OwlCore.Remoting.EventArgs;

namespace OwlCore.Remoting.Attributes
{
    /// <summary>
    /// Attribute used in conjuction with <see cref="MemberRemote"/>.
    /// Mark a property with this attribute to opt into remote method changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class RemotePropertyAttribute : Attribute, IPropertySetterInterceptor
    {
        /// <inheritdoc/>
        public bool OnException(Exception e)
        {
            ExceptionRaised?.Invoke(this, e);
            return true;
        }

        /// <inheritdoc/>
        public void OnExit()
        {
            Exited?.Invoke(this, System.EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
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
                if (frameMethod.Name != propertyInterceptionInfo.SetMethod.Name)
                    continue;

                // Check if the invoker was the OwlCore.Remoting lib between 1-8 frames back
                for (; i <= i + 8; i++)
                {
                    if (frames.Length == i)
                        break;

                    if (frames[i].GetMethod().Name == nameof(MemberRemote.MessageHandler_DataReceived))
                        return false;
                }

                break;
            }

            SetEntered?.Invoke(this, new PropertySetEnteredEventArgs(propertyInterceptionInfo, oldValue, newValue));
            return false;
        }

        /// <summary>
        /// Raised when the property is set.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IPropertySetterInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler<PropertySetEnteredEventArgs>? SetEntered;

        /// <summary>
        /// Raised when the property is finished setting and is about to exit.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IPropertySetterInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler? Exited;

        /// <summary>
        /// Raised when an exception occurs while setting the property.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IPropertySetterInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler<Exception>? ExceptionRaised;
    }
}
