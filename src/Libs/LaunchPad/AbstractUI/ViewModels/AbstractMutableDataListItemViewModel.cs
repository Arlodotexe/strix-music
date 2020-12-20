using System;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for items that are shown in a <see cref="AbstractMutableDataListViewModel"/>.
    /// </summary>
    public class AbstractMutableDataListItemViewModel : AbstractUIMetadataViewModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractMutableDataListItemViewModel"/>.
        /// </summary>
        /// <param name="metadata"></param>
        public AbstractMutableDataListItemViewModel(AbstractUIMetadata metadata)
            : base(metadata)
        {
            RequestRemoveCommand = new RelayCommand(RemoveSelf);
            RequestAddCommand = new RelayCommand(RequestAdd);

            VisibleIfIsAddItem = Id == "requestAddItem" ? Visibility.Visible : Visibility.Collapsed;
            CollapsedIfIsAddItem = Id == "requestAddItem" ? Visibility.Collapsed : Visibility.Visible;
        }

        private void RemoveSelf() => ItemRemoved?.Invoke(this, EventArgs.Empty);

        private void RequestAdd() => ItemAddRequested?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Raised when the user requests to remove the item.
        /// </summary>
        public event EventHandler? ItemRemoved;

        /// <summary>
        /// Raised when the user wants to add a new item.
        /// </summary>
        public event EventHandler? ItemAddRequested;

        /// <summary>
        /// Run this command to request the removal of this item from the containing list.
        /// </summary>
        public IRelayCommand RequestRemoveCommand { get; set; }

        /// <summary>
        /// Run this command to request a new item be added to the containing list.
        /// </summary>
        public IRelayCommand RequestAddCommand { get; set; }

        /// <summary>
        /// If the current data is for the item is used to request a new item, this returns <see cref="Visibility.Collapsed"/>.
        /// </summary>
        public Visibility CollapsedIfIsAddItem { get; }


        /// <summary>
        /// If the current data is for the item is used to request a new item, this returns <see cref="Visibility.Visible"/>.
        /// </summary>
        public Visibility VisibleIfIsAddItem { get; }
    }
}