using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.WinUI.Controls.Collections.Events
{
    public sealed class SelectionChangedEventArgs<T> : EventArgs
        where T : class
    {
        public SelectionChangedEventArgs(List<T> addedItems,List<T> removedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        public List<T> AddedItems { get; }
        public List<T> RemovedItems { get; }
    }
}
