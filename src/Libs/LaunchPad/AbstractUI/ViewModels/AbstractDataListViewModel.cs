using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel for the <see cref="AbstractDataList"/>.
    /// </summary>
    public class AbstractDataListViewModel : AbstractUIViewModelBase<AbstractDataList>
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
            Items = new ObservableCollection<AbstractUIMetadataViewModel>(_model.Items.Select(x=> new AbstractUIMetadataViewModel(x)));
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
        /// Checks the current display mode by name.
        /// </summary>
        /// <param name="name">The string representation of the <see cref="AbstractDataListPreferredDisplayMode"/> value.</param>
        /// <returns>A boolean indicating if the given string matches the current <see cref="PreferredDisplayMode"/>.</returns>
        public Visibility IsDisplayMode(string name)
        {
            if (!Enum.TryParse<AbstractDataListPreferredDisplayMode>(name, out var result))
                return Visibility.Collapsed;

            return result == PreferredDisplayMode ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}