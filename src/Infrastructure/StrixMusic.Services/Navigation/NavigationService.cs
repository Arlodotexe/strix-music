using System;
using System.Collections.Generic;

namespace StrixMusic.Services.Navigation
{
    /// <summary>
    /// An <see cref="INavigationService{T}"/> implmentation.
    /// </summary>
    public class NavigationService<T> : INavigationService<T>
    {
        private Dictionary<Type, T> _registeredPages = new Dictionary<Type, T>();

        /// <inheritdoc/>
        public event EventHandler<NavigateEventArgs<T>>? NavigationRequested;

        /// <inheritdoc/>
        public event EventHandler? BackRequested;

        /// <inheritdoc/>
        public void NavigateTo(Type type, bool overlay = false, object? args = null)
        {
            T page;
            if (_registeredPages.ContainsKey(type))
            {
                page = _registeredPages[type];
            }
            else
            {
                if (args != null)
                {
                    page = (T)Activator.CreateInstance(type, args);
                }
                else
                {
                    page = (T)Activator.CreateInstance(type);
                }
            }

            NavigateEventArgs<T> eventArgs = new NavigateEventArgs<T>(page, overlay);
            NavigationRequested?.Invoke(this, eventArgs);
        }

        /// <inheritdoc/>
        public void NavigateTo(T page, bool overlay = false)
        {
            NavigateEventArgs<T> eventArgs = new NavigateEventArgs<T>(page, overlay);
            NavigationRequested?.Invoke(this, eventArgs);
        }

        /// <inheritdoc/>
        public void RegisterCommonPage(Type type)
        {
            _registeredPages.Add(type, (T)Activator.CreateInstance(type));
        }

        /// <inheritdoc/>
        public void GoBack()
        {
            BackRequested?.Invoke(this, null);
        }
    }
}
