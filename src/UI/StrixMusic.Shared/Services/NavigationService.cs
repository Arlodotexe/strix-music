using System;
using System.Collections.Generic;
using StrixMusic.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Services
{
    /// <summary>
    /// An <see cref="INavigationService{T}"/> implmentation for <see cref="Control"/>.
    /// </summary>
    public class NavigationService : INavigationService<Control>
    {
        private Dictionary<Type, Control> _registeredPage = new Dictionary<Type, Control>();

        /// <inheritdoc/>
        public event EventHandler<Control>? NavigationRequested;

        /// <inheritdoc/>
        public void NavigateTo(Type type, object? args = null)
        {
            if (_registeredPage.ContainsKey(type))
            {
                NavigationRequested?.Invoke(this, _registeredPage[type]);
            }
            else
            {
                NavigationRequested?.Invoke(this, (Control)Activator.CreateInstance(type, args));
            }
        }

        /// <inheritdoc/>
        public void NavigateTo(Control control)
        {
            NavigationRequested?.Invoke(this, control);
        }

        /// <inheritdoc/>
        public void RegisterCommonPage(Type type)
        {
            _registeredPage.Add(type, (Control)Activator.CreateInstance(type));
        }
    }
}
