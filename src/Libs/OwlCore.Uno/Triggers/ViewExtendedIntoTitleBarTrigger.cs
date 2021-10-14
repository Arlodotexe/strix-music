using OwlCore.Uno.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace OwlCore.Uno.Triggers
{
    /// <summary>
    /// A state trigger that is active when the <see cref="CoreApplicationViewTitleBar"/> is visible.
    /// </summary>
    public class ViewExtendedIntoTitleBarTrigger : StateTriggerBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ViewExtendedIntoTitleBarTrigger"/>.
        /// </summary>
        public ViewExtendedIntoTitleBarTrigger()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var weakEvent = new WeakEventListener<ViewExtendedIntoTitleBarTrigger, CoreApplicationViewTitleBar, object>(this)
                {
                    OnEventAction = (instance, source, eventArgs) => instance.TitleBar_IsVisibleChanged(source, eventArgs),
                    OnDetachAction = (weakEventListener) => CoreApplication.GetCurrentView().TitleBar.IsVisibleChanged -= weakEventListener.OnEvent
                };

                CoreApplication.GetCurrentView().TitleBar.IsVisibleChanged += weakEvent.OnEvent;

                UpdateTrigger(CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the trigger is currently active.
        /// </summary>
        public bool IsTriggerActive { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to trigger when title bar is extended or not.
        /// </summary>
        public bool IsExtended { get; set; } = true;

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args) => UpdateTrigger(sender.ExtendViewIntoTitleBar);

        private void UpdateTrigger(bool isExtended)
        {
            IsTriggerActive = IsExtended == isExtended;
            SetActive(IsTriggerActive);
        }
    }
}
