using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls
{

    /// <summary>
    /// A base class for the root control that all shells implement.
    /// </summary>
    public abstract partial class Shell : UserControl
    {
        /// <summary>
        /// Defines options that can be used to modify the window host surrounding the shell, if present.
        /// </summary>
        public ShellWindowHostOptions WindowHostOptions { get; set; } = new(); 

        /// <summary>
        /// The backing dependency property for <see cref="Root"/>.
        /// </summary>
        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register(nameof(Root), typeof(IStrixDataRoot), typeof(Shell), new PropertyMetadata(null, (d, e) => ((Shell)d).OnRootChanged(e.OldValue as IStrixDataRoot, e.NewValue as IStrixDataRoot)));

        /// <summary>
        /// Fires when the <see cref="Root"/> is changed.
        /// </summary>
        protected virtual void OnRootChanged(IStrixDataRoot? oldValue, IStrixDataRoot? newValue)
        {
            SetValue(RootVmProperty, newValue is null ? null : (newValue as StrixDataRootViewModel ?? new StrixDataRootViewModel(newValue)));
        }

        /// <summary>
        /// The <see cref="IStrixDataRoot"/> to use for getting data.
        /// </summary>
        public IStrixDataRoot? Root
        {
            get => (IStrixDataRoot?)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="RootVm"/>.
        /// </summary>
        public static readonly DependencyProperty RootVmProperty =
            DependencyProperty.Register(nameof(RootVm), typeof(StrixDataRootViewModel), typeof(Shell), new PropertyMetadata(null));

        /// <summary>
        /// A ViewModel wrapper for all merged core data.
        /// </summary>
        public StrixDataRootViewModel? RootVm => (StrixDataRootViewModel?)GetValue(RootVmProperty);
    }
}
