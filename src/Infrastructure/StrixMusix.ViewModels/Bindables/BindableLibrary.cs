using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IPlayableCollectionGroup"/> to be parsed as a library.
    /// </summary>
    public class BindableLibrary : ObservableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableLibrary"/> class.
        /// </summary>
        /// <param name="library">The <see cref="IPlayableCollectionGroup"/> representing the library.</param>
        public BindableLibrary(IPlayableCollectionGroup library)
        {
            Library = library;
        }

        /// <summary>
        /// The <see cref="IPlayableCollectionGroup"/> representing the library content.
        /// </summary>
        public IPlayableCollectionGroup Library { get; set; }
    }
}
