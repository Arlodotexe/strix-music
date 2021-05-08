using System;
using System.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for items that are shown in a <see cref="AbstractDataListViewModel"/>.
    /// </summary>
    [Bindable(true)]
    public class AbstractDataListItemViewModel : AbstractUIMetadataViewModel
    {
        private readonly AbstractDataListViewModel _parent;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractDataListItemViewModel"/>.
        /// </summary>
        /// <param name="metadata">The metadata for this item.</param>
        /// <param name="parent">The parent that this item belongs to.</param>
        public AbstractDataListItemViewModel(AbstractUIMetadata metadata, AbstractDataListViewModel parent)
            : base(metadata)
        {
            _parent = parent;
            RequestRemoveCommand = new RelayCommand(RemoveSelf);
            RequestAddCommand = new RelayCommand(RequestAdd);

            IsAddItem = Id == "requestAddItem";
        }

        private void RemoveSelf() => ItemRemoved?.Invoke(this, EventArgs.Empty);

        private void RequestAdd()
        {
            if (IsAddItem)
                ItemAddRequested?.Invoke(this, EventArgs.Empty);
        }

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

        /// <inheritdoc cref="AbstractDataList.IsUserEditingEnabled"/>
        public bool IsUserEditingEnabled => _parent.IsUserEditingEnabled;

        /// <summary>
        /// If the current item is used to request a new item.
        /// </summary>
        public bool IsAddItem { get; }
    }
}