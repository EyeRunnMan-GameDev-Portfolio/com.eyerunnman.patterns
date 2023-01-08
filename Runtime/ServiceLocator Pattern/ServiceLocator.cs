using com.eyerunnman.patterns;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    /// <summary>
    /// Simple service locator for <see cref="IGameService"/> instances.
    /// </summary>
    public class ServiceLocator
    {
        private ServiceLocator() { }

        /// <summary>
        /// currently registered services.
        /// </summary>
        private readonly Dictionary<string, IGameService> services = new Dictionary<string, IGameService>();

        /// <summary>
        /// Gets the currently active service locator instance.
        /// </summary>
        public static ServiceLocator Current { 

            get{
                if (_current != null)
                    return _current;

                Initiailze();
                return _current;
            }
        }

        private static ServiceLocator _current;


        /// <summary>
        /// Initalizes the service locator with a new instance.
        /// </summary>
        private static void Initiailze()
        {
            _current = new ServiceLocator();
        }

        /// <summary>
        /// Gets the service instance of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to lookup.</typeparam>
        /// <returns>The service instance.</returns>
        public T Get<T>() where T : IGameService
        {
            string key = typeof(T).Name;
            if (!services.ContainsKey(key))
            {
                throw ServiceNotRegisteredException<T>();
            }

            return (T)services[key];
        }

        /// <summary>
        /// Registers the service with the current service locator.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="service">service instance.</param>
        /// <param name="useCurrent">strictly use Current Service</param>
        public void Register<T>(T service,bool overrideService=false) where T : IGameService
        {
            string key = typeof(T).Name;
            if (services.ContainsKey(key))
            {

                if (overrideService)
                {
                    // if the object is exactly same skip
                    if(service.Equals(services[key]))
                    {
                        return;
                    }

                    //unregister previous service 
                    Unregister<T>();

                    //register current service 
                    Register<T>(service);
                }
                else
                {
                    service.OnRegisterError(errMsg: "Fatal : regestering a service of type without overriding");
                }

                Debug.LogError($"Attempted to register service of type {key} which is already registered with the {GetType().Name}.");
                return;
            }

            services.Add(key, service);

            service.OnRegister();
        }

        /// <summary>
        /// Unregisters the service from the current service locator.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        public void Unregister<T>() where T : IGameService
        {
            string key = typeof(T).Name;
            if (!services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to unregister service of type {key} which is not registered with the {GetType().Name}.");
                return;
            }

            services[key].OnUnRegister();

            services.Remove(key);

        }


        public static Exception ServiceNotRegisteredException<T>()
        {
            throw new System.Exception("Required Service: " + typeof(T).Name + " -> Not Registered");
        }
    }
}