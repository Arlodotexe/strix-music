using System;
using System.Diagnostics;
using System.Reflection;
using Cauldron.Interception;

namespace OwlCore.Remoting.Attributes
{
    /// <summary>
    /// Attribute used in conjuction with <see cref="MemberRemote"/>.
    /// Mark any member with this to control the data flow direction when changes are relayed remotely.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RemoteMethodAttribute : Attribute, IMethodInterceptor, IMethodInterceptorOnExit
    {
        private Type? _declaringType;
        private MethodBase? _methodBase;
        private object? _instance;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodAttribute"/>.
        /// </summary>
        /// <param name="direction">The remoting direction to use when relaying class changes remotely.</param>
        public RemoteMethodAttribute(RemotingDirection direction)
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

            Debug.WriteLine("Entered: " + methodbase.Name);
        }

        /// <inheritdoc/>
        public bool OnException(Exception e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnExit()
        {
        }

        /// <inheritdoc/>
        public object OnExit(Type returnType, object returnValue)
        {
            Debug.WriteLine("Exiting: " + _methodBase?.Name ?? "null");
            return returnValue;
        }
    }
}
