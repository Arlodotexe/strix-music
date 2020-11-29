﻿using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the root page of the shell.
    /// </summary>
    public class RootPageSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="HomeView"/> template.
        /// </summary>
        public DataTemplate? HomeViewTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case PlayableCollectionGroupViewModel _:
                    return HomeViewTemplate!;
                default:
                    return HomeViewTemplate!;
            }
        }
    }
}
