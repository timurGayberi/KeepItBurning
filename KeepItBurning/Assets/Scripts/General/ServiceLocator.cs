using System;
using UnityEngine;
using System.Collections.Generic;

namespace General
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();
        
        public static void RegisterService<T>(T service) where T : class
        {
            if (service == null)
            {
                return;
            }

            var serviceType = typeof(T);
            
            if (Services.ContainsKey(serviceType))
            {
            }
            
            Services[serviceType] = service;
        }
        
        public static T GetService<T>() where T : class
        {
            var serviceType = typeof(T);
            if (Services.TryGetValue(serviceType, out object service))
            {
                return (T)service; 
            }
            
            throw new InvalidOperationException($"Service of type {serviceType.Name} not found in Locator. Check Bootstrapper execution order.");
        }
        
        public static bool IsServiceRegistered<T>() where T : class
        {
            return Services.ContainsKey(typeof(T));
        }

        public static void ClearServices()
        {
            Services.Clear();
        }
    }
}