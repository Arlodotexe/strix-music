using System;
using System.Reflection;
using Cauldron.Interception;
using OwlCore.Remoting.EventArgs;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace OwlCore.Remoting.Attributes
{
    /// <summary>
    /// Attribute used in conjuction with <see cref="MemberRemote"/>.
    /// Mark any member with this to control the data flow direction when changes are relayed remotely.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RemoteMethod_v1Attribute : Attribute, IMethodInterceptor, IMethodInterceptorOnExit 
    {
        private Type? _declaringType;
        private MethodBase? _methodBase;
        private object? _instance;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethod_v1Attribute"/>.
        /// </summary>
        /// <param name="direction">The remoting direction to use when relaying class changes remotely.</param>
        public RemoteMethod_v1Attribute(RemotingDirection direction)
        {
            Direction = direction;
        }

        /// <inheritdoc cref="RemotingDirection"/>
        public RemotingDirection Direction { get; }

        /// <inheritdoc/>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            _declaringType = declaringType;
            _instance = instance;
            _methodBase = methodbase;

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

        public object OnExit(Type returnType, object returnValue)
        {
            System.Diagnostics.Debug.WriteLine(returnType?.FullName ?? "null" + ", " + returnValue?.GetType() ?? "null");
            return returnValue!;
        }

        /// <summary>
        /// Raised when the attached method is fired.
        /// </summary>
        public event EventHandler<MethodEnteredEventArgs>? Entered;

        /// <summary>
        /// Raised when 
        /// </summary>
        public event EventHandler? Exited;

        /// <summary>
        /// Raised when an exception occurs in the method.
        /// </summary>
        public event EventHandler<Exception>? ExceptionRaised;
    }

    [PSerializable]
    public class RemoteMethodAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            base.OnEntry(args);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            base.OnExit(args);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            base.OnException(args);
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            base.OnSuccess(args);
        }
    }
}
