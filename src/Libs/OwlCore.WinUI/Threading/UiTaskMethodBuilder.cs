using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace OwlCore.WinUI.Threading
{
    /// <summary>
    /// Builds a new <see cref="UiTask"/>.
    /// </summary>
    public class UiTaskMethodBuilder
    {
        private readonly CoreDispatcher _dispatcher;

        /// <summary>
        /// Creates a new instance of <see cref="UiTaskMethodBuilder"/>.
        /// </summary>
        /// <param name="dispatcher"></param>
        public UiTaskMethodBuilder(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Starts the async machine.
        /// </summary>
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            if (!_dispatcher.HasThreadAccess)
            {
                var action = new Action(stateMachine.MoveNext);

                _ = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            }
            else
            {
                stateMachine.MoveNext();
            }
        }

        /// <summary>
        /// Creates a singleton of <see cref="UiTaskMethodBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static UiTaskMethodBuilder Create()
        {
            Guard.IsNotNull(Window.Current);
            return new UiTaskMethodBuilder(Window.Current.Dispatcher);
        }

        /// <summary>
        /// This one is pretty obscure, I’m not even sure when it’s supposed to be called. We’re not going to need it so we’ll leave it empty.
        /// </summary>
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        /// <summary>
        /// Called at the end of the async method, to set the result. We can simply map to the TaskCompletionSource.
        /// </summary>
        public void SetResult()
        {
            Task.Promise.SetResult(new object());
        }

        /// <summary>
        /// Called at the end of the async method, to set an exception. We can simply map to the TaskCompletionSource.
        /// </summary>
        /// <param name="exception"></param>
        public void SetException(Exception exception)
        {
            Task.Promise.SetException(exception);
        }

        /// <summary>
        /// Exposes the instance of our custom UI task that will be returned by the async method
        /// </summary>
        public UiTask Task { get; } = new UiTask();

        /// <summary>
        /// Called to set the completion state.
        /// </summary>
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter,
            ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(ResumeAfterAwait(stateMachine));
        }

        /// <summary>
        /// Called to set the completion state, if the awaiter implements ICriticalNotifyCompletion.
        /// </summary>
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter,
            ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(ResumeAfterAwait(stateMachine));
        }

        private Action ResumeAfterAwait<TStateMachine>(TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            return () =>
            {
                if (!_dispatcher.HasThreadAccess)
                {
                    var action = new Action(stateMachine.MoveNext);
                    _ = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
                }
                else
                {
                    stateMachine.MoveNext();
                }
            };
        }
    }
}
