using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore;
using LaunchPad.Extensions;
using Windows.UI.Xaml;

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
            _ = NavigateToTypeAsync(type, overlay, args);
        }

        /// <inheritdoc/>
        public void NavigateTo(T page, bool overlay = false)
        {
            _ = NavigateToInstanceAsync(page, overlay);
        }

        /// <inheritdoc/>
        public void RegisterCommonPage(Type type)
        {
            using (Threading.PrimaryContext)
            {
                _registeredPages.Add(type, (T)Activator.CreateInstance(type));
            }
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

        private async Task NavigateToInstanceAsync(T page, bool overlay = false)
        {
            await Window.Current.Dispatcher.SwitchToUi();

            var eventArgs = new NavigateEventArgs<T>(page, overlay);
            NavigationRequested?.Invoke(this, eventArgs);
        }

        private async Task NavigateToTypeAsync(Type type, bool overlay = false, params object[] args)
        {
            await Window.Current.Dispatcher.SwitchToUi();

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
    }
}
