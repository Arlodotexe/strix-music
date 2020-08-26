using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="ILibrary"/>.
    /// </summary>
    public class BindableLibrary : BindableCollectionGroup
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BindableLibrary"/> class.
        /// </summary>
        /// <param name="library">The <see cref="ILibrary"/> to wrap.</param>
        public BindableLibrary(ILibrary library)
            : base(library)
        {
        }
    }
}
