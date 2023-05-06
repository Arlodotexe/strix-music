using System;

namespace StrixMusic.Sdk.WinUI.Controls.Collections.Events
{
    /// <summary>
    /// EventArgs for <see cref="Abstract.CollectionControl{TData, TItem}.SelectionChanged"/>
    /// </summary>
    /// <typeparam name="T">The type of the selected item(s).</typeparam>
    public sealed class SelectionChangedEventArgs<T> : EventArgs
        where T : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="SelectionChangedEventArgs{T}"/>.
        /// </summary>
        /// <param name="selectedItem">The item that was selected.</param>
        public SelectionChangedEventArgs(T selectedItem)
        {
            SelectedItem = selectedItem;
        }

        /// <summary>
        /// The item that was selected.
        /// </summary>
        public T SelectedItem { get; }
    }
}
