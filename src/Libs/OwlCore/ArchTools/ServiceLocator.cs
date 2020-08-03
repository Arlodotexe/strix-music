using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 649

namespace OwlCore.ArchTools
{
    /// <summary>
    /// Locates registered objects.
    /// </summary>
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "Constant can be toggled by manually changing code")]
    public class ServiceLocator
    {
        /// <summary>
        /// If true, registering a service more than once will replace the existing one
        /// </summary>
        public const bool REPLACE_EXISTING = true;

        /// <summary>
        /// Fires when an internal errors occurs in ServiceLocator
        /// </summary>
        public event EventHandler<string>? ErrorOccured;

        private static readonly Func<object> _noFactory = () => new object();

        private static readonly object _syncRoot = new object();
        private static ServiceLocator _instance = new ServiceLocator();

        private Dictionary<Type, Func<object>> _objectFactories;
        private Dictionary<Type, object> _createdInstances;

        /// <summary>
        /// The static instance of the <see cref="ServiceLocator"/>
        /// </summary>
        public static ServiceLocator Instance
        {
            get
            {
                lock (_syncRoot)
                {
                    return _instance;
                }
            }

            set
            {
                lock (_syncRoot)
                    _instance = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ServiceLocator"/> as type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <returns>The <see cref="ServiceLocator"/> cast to <typeparamref name="T"/></returns>
        public static T InstanceAs<T>()
            where T : ServiceLocator
        {
            return (T)Instance;
        }

        /// <summary>
        /// Initializes the <see cref="ServiceLocator"/>
        /// </summary>
        protected ServiceLocator()
        {
            _objectFactories = new Dictionary<Type, Func<object>>();
            _createdInstances = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Register an object with the service locator
        /// </summary>
        /// <typeparam name="T">The type of the object to register</typeparam>
        /// <param name="data">Object to register</param>
        public void Register<T>(T data)
        {
            Register<T>(_noFactory, data);
        }

        /// <summary>
        /// Registers a <typeparamref name="T"/> on the <see cref="ServiceLocator"/>, with a <paramref name="factory"/> to re-create the object each time
        /// </summary>
        /// <typeparam name="T">The type of the object to register</typeparam>
        /// <param name="factory">Factory that re-creates the object each time the service is resolved</param>
        public void Register<T>(Func<object> factory)
        {
            Register<T>(factory, null);
        }

        /// <summary>
        /// Registers a <typeparamref name="T"/> on the <see cref="ServiceLocator"/>, with a <paramref name="factory"/> to re-create the object each time
        /// </summary>
        /// <typeparam name="T">The type of the object to register</typeparam>
        /// <param name="factory">Factory that re-creates the object each time the service is resolved</param>
        /// <param name="initialValue">The initial stored value, for when no factory is specified. If null, factories are relied on.</param>
        private void Register<T>(Func<object> factory, object? initialValue)
        {
            var type = typeof(T);

            if (type == null)
                throw new ArgumentException("Type is null.", nameof(type));

            if (factory == null)
                throw new ArgumentException("Factory is null.", nameof(factory));

            if (_objectFactories.ContainsKey(type))
            {
                ErrorOccured?.Invoke(this, "Type is already registered in service locator. Type : " + type);
                if (REPLACE_EXISTING)
                {
                    _objectFactories[type] = factory;
                }

                return;
            }

            if (factory != _noFactory)
                _objectFactories.Add(type, factory);

            if (_createdInstances.ContainsKey(type))
            {
                ErrorOccured?.Invoke(this, "Type is already registered in service locator. Type : " + type);
                if (REPLACE_EXISTING)
                {
                    if (initialValue != null)
                        _createdInstances[type] = initialValue;
                }

                return;
            }

            if (initialValue != null)
                _createdInstances.Add(type, initialValue);
        }

        /// <summary>
        /// Gets a registered service, throws if it hasn't been registers
        /// </summary>
        /// <typeparam name="T">The type of the registered service to look for</typeparam>
        /// <returns>The registered service of type <typeparamref name="T"/></returns>
        public T Resolve<T>()
        {
            var type = typeof(T);
            if (_createdInstances.TryGetValue(type, out object instance))
                return (T)instance;

            if (!_objectFactories.ContainsKey(type))
                throw new ArgumentException("Type is not registered.");

            var fact = _objectFactories[type];

            var obj = fact();
            _createdInstances.Add(type, obj);

            return (T)obj;
        }

        /// <summary>
        /// Gets a registered service
        /// </summary>
        /// <typeparam name="T">The type of the registered service to look for</typeparam>
        /// <param name="obj">The resolved service</param>
        /// <returns><see cref="bool"/> indicating if the service is registered</returns>
        public bool TryResolve<T>(out T obj)
        {
            Type type = typeof(T);
            if (_createdInstances.TryGetValue(type, out object e))
            {
                obj = (T)e;
                return true;
            }

            if (!_objectFactories.ContainsKey(type))
            {
                // Todo: This can return null if obj is a class
                obj = default!;
                return false;
            }

            object instance = _objectFactories[type]();
            _createdInstances.Add(type, instance);

            obj = (T)instance;
            return true;
        }

        /// <summary>
        /// Removes all registered services
        /// </summary>
        public void ClearRegisteredItems()
        {
            _objectFactories.Clear();
            _createdInstances.Clear();
        }
    }
}
