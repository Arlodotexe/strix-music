using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    /// <summary>
    /// A ViewModel for an <see cref="Controls.Pages.GrooveHomePage"/>.
    /// </summary>
    public class GrooveHomePageViewModel : ObservableObject
    {
        private LibraryViewModel? _libraryViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveHomePageViewModel"/> class.
        /// </summary>
        public GrooveHomePageViewModel()
        {
        }

        /// <summary>
        /// The <see cref="LibraryViewModel"/> inside this ViewModel on display.
        /// </summary>
        public LibraryViewModel? Library
        {
            get => _libraryViewModel;
            set => SetProperty(ref _libraryViewModel, value);
        }
    }
}
