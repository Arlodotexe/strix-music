using OwlCore.Extensions;
using System;
using System.Reflection;
using static OwlCore.Extensions.MethodInfoExtensions;

namespace OwlCore.MemberInterception
{
    /// <summary>
    /// Intercepts when a method is called.
    /// </summary>
    public class MethodIntercept : IDisposable
    {
        private readonly object _instance;
        private readonly MethodReplacementState _methodReplacementState;

        /// <summary>
        /// Creates a new instance of <see cref="MethodIntercept"/>.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to inject into.</param>
        /// <param name="instance">The instance to call the given <see cref="MethodInfo"/> on.</param>
        public MethodIntercept(MethodInfo methodInfo, object instance)
        {
            OriginalMethodInfo = methodInfo;
            _instance = instance;

            WrappedMethodInfo = typeof(MethodIntercept).GetMethod(nameof(ExecuteWrapped));

            // Replace the original method with the wrapped method
            // Potential issues with JIT / AOT?
            _methodReplacementState = OriginalMethodInfo.SwapPointerWith(WrappedMethodInfo);

            // Since the method pointers were swapped, swap these so we call the correct one.
            OriginalMethodInfo = WrappedMethodInfo;
        }

        /// <summary>
        /// The original <see cref="MethodInfo"/> given to the ctor.
        /// </summary>
        public MethodInfo OriginalMethodInfo { get; set; }

        /// <summary>
        /// The <see cref="MethodInfo"/> that wraps both the <see cref="OriginalMethodInfo"/> and injection action. 
        /// </summary>
        public MethodInfo WrappedMethodInfo { get; set; }

        /// <summary>
        /// Raised when the given <see cref="MethodInfo"/> is executed.
        /// </summary>
        public event EventHandler<object[]>? MethodExecuted;

        private object ExecuteWrapped(params object[] parameters)
        {
            // Call the injected action
            MethodExecuted?.Invoke(this, parameters);

            // Call the original method.
            return OriginalMethodInfo.Invoke(_instance, parameters);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _methodReplacementState.Restore();
        }
    }

    public class PropertyIntercept : IDisposable
    {
        public PropertyIntercept(PropertyInfo propertyInfo, object instance)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
