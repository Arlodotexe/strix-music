using System;
using System.Collections.Generic;
using System.Linq;

namespace StrixMusic.Sdk.Services.Navigation
{
    /// <summary>
    /// An <see cref="INavigationService{T}"/> implementation.
    /// </summary>
    public class NavigationService<T> : INavigationService<T>
    {
        private readonly Dictionary<Type, T> _registeredPages = new Dictionary<Type, T>();

        /// <inheritdoc/>
        public event EventHandler<NavigateEventArgs<T>>? NavigationRequested;

        /// <inheritdoc/>
        public event EventHandler? BackRequested;

        /// <inheritdoc/>
        public void NavigateTo(Type type, bool overlay = false, params object[] args)
        {
            T page;
            if (_registeredPages.ContainsKey(type))
            {
                page = _registeredPages[type];
            }
            else
            {
                if (args.Any())
                {
                    page = (T)Activator.CreateInstance(type, args);
                }
                else
                {
                    page = (T)Activator.CreateInstance(type);
                }
            }

            var eventArgs = new NavigateEventArgs<T>(page, overlay);
            NavigationRequested?.Invoke(this, eventArgs);
        }

        /// <inheritdoc/>
        public void NavigateTo(T page, bool overlay = false)
        {
            var eventArgs = new NavigateEventArgs<T>(page, overlay);
            NavigationRequested?.Invoke(this, eventArgs);
        }

        /// <inheritdoc/>
        public void RegisterCommonPage(Type type)
        {
            _registeredPages.Add(type, (T)Activator.CreateInstance(type));
        }

        /// <inheritdoc />
        public void RegisterCommonPage(T type)
        {
            if (type != null)
            {
                _registeredPages.Add(type.GetType(), type);
            }
        }

        /// <inheritdoc/>
        public void GoBack()
        {
            BackRequested?.Invoke(this, null);
        }
    }
}
