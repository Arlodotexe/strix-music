using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// A base class containing the implementation for handling merged data.
    /// </summary>
    public class ObservableMergeableObject<T> : ObservableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableMergeableObject{T}"/> class.
        /// </summary>
        public ObservableMergeableObject()
        {
            _mergedData = new ObservableCollection<T>();
            MergedData = new ReadOnlyObservableCollection<T>(_mergedData);
        }

        /// <summary>
        /// Adds a matched, less preferred version of the this item from a different data source.
        /// </summary>
        /// <param name="data"></param>
        internal void AddMergedData(T data)
        {
            _mergedData.Add(data);
        }

        private ObservableCollection<T> _mergedData;

        /// <summary>
        /// A collection of matched, less preferred versions of this item from other sources.
        /// </summary>
        public ReadOnlyObservableCollection<T> MergedData { get; }
    }
}
