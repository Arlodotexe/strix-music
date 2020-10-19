using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LaunchPad.Extensions.System.Threading.Tasks
{
    public static partial class TaskExtensions
    {
        /// <summary>
        /// Invokes a task on the UI thread.
        /// </summary>
        /// <param name="task">The task to invoke.</param>
        public static async Task InvokeOnUI(this Task task)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
         {
             await task;
         });
        }
    }
}
