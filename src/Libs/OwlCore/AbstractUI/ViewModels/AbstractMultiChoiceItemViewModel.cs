using System;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for items that are shown in a <see cref="AbstractDataListViewModel"/>.
    /// </summary>
    public class AbstractMultiChoiceItemViewModel : AbstractUIMetadataViewModel
    {
        private bool _isSelected;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractDataListItemViewModel"/>.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="groupId">A unique identifier that can be used to group together <see cref="AbstractMultiChoiceItemViewModel"/>s.</param>
        public AbstractMultiChoiceItemViewModel(AbstractUIMetadata metadata, string groupId)
            : base(metadata)
        {
            GroupId = groupId;
            ItemSelectedCommand = new RelayCommand(OnSelectItem);
        }

        private void OnSelectItem()
        {
            ItemSelected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// A unique identifier that can be used to group this item with other <see cref="AbstractMultiChoiceItemViewModel"/>s.
        /// </summary>
        public string GroupId { get; }

        /// <summary>
        /// True if this multi choice item is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// Fires when the user requests to remove the item.
        /// </summary>
        public event EventHandler? ItemSelected;

        /// <summary>
        /// Run this command to request the removal of this item.
        /// </summary>
        public IRelayCommand ItemSelectedCommand { get; set; }
    }
}