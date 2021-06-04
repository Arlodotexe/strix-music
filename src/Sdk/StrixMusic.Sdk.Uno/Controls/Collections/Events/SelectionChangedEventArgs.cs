using System;

namespace StrixMusic.Sdk.Uno.Controls.Collections.Events
{
    /// <summary>
    /// Event args used for when an item is selected in a list.
    /// </summary>
    /// <typeparam name="T">The type of item being selected.</typeparam>
    public class SelectionChangedEventArgs<T> : EventArgs
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionChangedEventArgs{T}"/> class.
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
