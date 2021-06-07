using System;

namespace StrixMusic.Sdk.Uno.Controls.Collections.Events
{
    public class SelectionChangedEventArgs<T> : EventArgs
        where T : class
    {
        public SelectionChangedEventArgs(T selectedItem)
        {
            SelectedItem = selectedItem;
        }

        public T SelectedItem { get; }
    }
}
