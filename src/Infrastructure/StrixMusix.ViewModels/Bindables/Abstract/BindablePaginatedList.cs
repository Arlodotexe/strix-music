using System.Collections.Generic;
using System.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Interfaces.Abstract;

namespace StrixMusic.ViewModels.Bindables.Abstract
{
    /// <summary>
    /// A bindable wrapper for <see cref="IPaginatedList"/>s.
    /// </summary>
    /// <typeparam name="T">The paginiation</typeparam>
    public class BindablePaginatedList<T, TItem> : ObservableObject
        where T : IPaginatedList<TItem>
    {
        private Mutex _mutex = new Mutex();
        private T _pagedList;
        private bool _loadingNextPage;
        private bool _loadingPreviousPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindablePaginatedList{T, TItem}"/> class.
        /// </summary>
        /// <param name="list">The <see cref="IPaginatedList{T}"/> to display</param>
        public BindablePaginatedList(T list)
        {
            _pagedList = list;
        }

        /// <summary>
        /// Gets a value indicating whether or not the next page is being loaded.
        /// </summary>
        public bool LoadingNextPage
        {
            get => _loadingNextPage;
            set
            {
                if (SetProperty(ref _loadingNextPage, value))
                {
                    OnPropertyChanged(nameof(LoadingPage));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the previous page is being loaded.
        /// </summary>
        public bool LoadingPreviousPage
        {
            get => _loadingPreviousPage;
            set
            {
                if (SetProperty(ref _loadingPreviousPage, value))
                {
                    OnPropertyChanged(nameof(LoadingPage));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a page is being loaded.
        /// </summary>
        public bool LoadingPage => _loadingNextPage || _loadingPreviousPage;

        /// <summary>
        /// The loaded Items in the view.
        /// </summary>
        public List<TItem> Items { get; } = new List<TItem>();

        /// <summary>
        /// Loads the contents of the next page to <see cref="Items"/>.
        /// </summary>
        public async void LoadNextItems()
        {
            _mutex.WaitOne();
            LoadingNextPage = true;
            await _pagedList.LoadNextPage();
            LoadingNextPage = false;
            _mutex.ReleaseMutex();
        }

        /// <summary>
        /// Loads the contents of the next page to <see cref="Items"/>.
        /// </summary>
        public async void LoadPreviousItems()
        {
            _mutex.WaitOne();
            LoadingPreviousPage = true;
            await _pagedList.LoadPreviousPage();
            LoadingPreviousPage = false;
            _mutex.ReleaseMutex();
        }
    }
}
