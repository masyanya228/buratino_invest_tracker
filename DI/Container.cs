using Buratino.Entities.Abstractions;
using Buratino.Models.DomainService;
using Buratino.Models.DomainService.DomainStructure;


using System;

namespace Buratino.DI
{
    public class Container
    {
        internal static IServiceProvider _serviceProvider = null;

        public static bool IsReady { get => _serviceProvider is not null; }

        /// <summary>
        /// Configure ServiceActivator with full serviceProvider
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Create a scope where use this ServiceActivator
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IServiceScope GetScope(IServiceProvider serviceProvider = null)
        {
            var provider = serviceProvider ?? _serviceProvider;
            return provider?
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
        }

        public static IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }

        public static object Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }
        
        public static T Resolve<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public static IDomainService<T> ResolveDomainService<T>() where T : IEntityBase
        {
            return Resolve<IDomainService<T>>();
        }

        public static object ResolveDomainService(Type type)
        {
            var genericType = typeof(IDomainService<>).MakeGenericType(type);
            return Resolve(genericType);
        }

        public static ObjectDomainService ResolveObjectDomainService(Type type)
        {
            return new ObjectDomainService(ResolveDomainService(type));
        }
    }
}
