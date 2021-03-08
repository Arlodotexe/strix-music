using System;
using System.Collections.ObjectModel;
using System.Linq;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel for the <see cref="AbstractDataList"/>.
    /// </summary>
    public class AbstractDataListViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractDataList _model;

        /// <summary>
        /// Initializes a new instance of see <see cref="AbstractDataListViewModel"/>.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractDataListViewModel(AbstractDataList model)
            : base(model)
        {
            _model = model;
            Items = new ObservableCollection<AbstractUIMetadataViewModel>(_model.Items.Select(x => new AbstractUIMetadataViewModel(x)));
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractDataListViewModel"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIMetadataViewModel this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public ObservableCollection<AbstractUIMetadataViewModel> Items { get; }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode => _model.PreferredDisplayMode;

        /// <summary>
        /// If true, the <see cref="PreferredDisplayMode"/> is a list.
        /// </summary>
        public bool IsList => PreferredDisplayMode == AbstractDataListPreferredDisplayMode.List;

        /// <summary>
        /// If true, the <see cref="PreferredDisplayMode"/> is a grid.
        /// </summary>
        public bool IsGrid => PreferredDisplayMode == AbstractDataListPreferredDisplayMode.Grid;
    }
}