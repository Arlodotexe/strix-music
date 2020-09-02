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
        public event EventHandler<T>? NavigationRequested;

        /// <inheritdoc/>
        public void NavigateTo(Type type, object? args = null)
        {
            if (_registeredPages.ContainsKey(type))
            {
                NavigationRequested?.Invoke(this, _registeredPages[type]);
            }
            else
            {
                if (args != null)
                {
                    NavigationRequested?.Invoke(this, (T)Activator.CreateInstance(type, args));
                } else
                {
                    NavigationRequested?.Invoke(this, (T)Activator.CreateInstance(type));
                }
            }
        }

        /// <inheritdoc/>
        public void NavigateTo(T control)
        {
            NavigationRequested?.Invoke(this, control);
        }

        /// <inheritdoc/>
        public void RegisterCommonPage(Type type)
        {
            _registeredPages.Add(type, (T)Activator.CreateInstance(type));
        }
    }
}
