namespace OwlCore.ArchTools
{
    /// <summary>
    /// A <c>struct</c> that automatically resolves a service via <see cref="ServiceLocator"/>, without assigning a value.
    /// </summary>
    /// <typeparam name="T">Service to resolve</typeparam>
    public struct LazyService<T>
    {
        private T _service;

        /// <summary>
        /// Returns true if the value is initialized from the ServiceLocator, false otherwise.
        /// </summary>
        public bool IsInitialized { get; set; }

        /// <summary>
        /// Returns the service if is initialized, false otherwise
        /// </summary>
        public T Value
        {
            get
            {
                if (_service == null && !IsInitialized)
                    IsInitialized = ServiceLocator.Instance.TryResolve(out _service);

                return _service;
            }
        }
    }
}