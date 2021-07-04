using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OwlCore.Extensions
{
    /// <summary>
    /// Extension methods that operate on <see cref="MethodInfo"/>
    /// </summary>
    /// <remarks>Modified from <see href="https://stackoverflow.com/a/55026523/5953220"/></remarks>
    public static partial class MethodInfoExtensions
    {
        /// <summary>
        /// Swaps the pointer of two <see cref="MethodInfo"/>s and returns a <see cref="MethodReplacementState"/> that can restore it.
        /// </summary>
        /// <param name="methodToReplace">The method to replace.</param>
        /// <param name="methodToInject">The new method to point to.</param>
        /// <returns><see cref="MethodReplacementState"/> that can be used to restore the method pointers.</returns>
        public static unsafe MethodReplacementState SwapPointerWith(this MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            #if DEBUG
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);
            #endif

            MethodReplacementState state;

            IntPtr tar = methodToReplace.MethodHandle.Value;
            if (!methodToReplace.IsVirtual)
            {
                tar += 8;
            }
            else
            {
                var index = (int)(((*(long*)tar) >> 32) & 0xFF);
                var classStart = *(IntPtr*)(methodToReplace.DeclaringType.TypeHandle.Value + (IntPtr.Size == 4 ? 40 : 64));
                tar = classStart + IntPtr.Size * index;
            }

            var inj = methodToInject.MethodHandle.Value + 8;

#if DEBUG
            tar = *(IntPtr*)tar + 1;
            inj = *(IntPtr*)inj + 1;
            state.Location = tar;
            state.OriginalValue = new IntPtr(*(int*)tar);

            *(int*)tar = *(int*)inj + (int)(long)inj - (int)(long)tar;
#else
            state.Location = tar;
            state.OriginalValue = *(IntPtr*)tar;
            * (IntPtr*)tar = *(IntPtr*)inj;
#endif

            return state;
        }

        /// <summary>
        /// Holds the swapped pointers created as a result of using <see cref="SwapPointerWith(MethodInfo, MethodInfo)"/>.
        /// </summary>
        public struct MethodReplacementState : IDisposable
        {
            internal IntPtr Location;
            internal IntPtr OriginalValue;

            /// <summary>
            /// Restores the method pointers.
            /// </summary>
            public void Dispose() => Restore();

            /// <summary>
            /// Restores the method pointers.
            /// </summary>
            public unsafe void Restore()
            {
#if DEBUG
                *(int*)Location = (int)OriginalValue;
#else
                *(IntPtr*)Location = OriginalValue;
#endif
            }
        }
    }
}
