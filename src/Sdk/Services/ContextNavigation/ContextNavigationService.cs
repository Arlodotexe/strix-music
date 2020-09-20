using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrixMusic.Sdk.Services.ContextNavigation
{
    /// <inheritdoc />
    public class ContextNavigationService<T> : IContextNavigationService<T>
    {
        /// <inheritdoc />
        public event EventHandler<ContextNavigateEventArgs<T>>? NavigationRequested;

        /// <inheritdoc />
        public void RequestNavigation(string coreName, string payload)
        {
            throw new NotImplementedException();
        }
    }
}
