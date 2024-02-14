using Buratino.Entities.Abstractions;
using Buratino.Models.DomainService;
using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Xtensions;

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

        public static T Resolve<T>(string key = null)
        {
            if (key is null)
                return _serviceProvider.GetService<T>();
            else
                return _serviceProvider.GetKeyedService<T>(key);
        }

        public static object Resolve(Type type, string key = null)
        {
            if (key is null)
                return _serviceProvider.GetService(type);
            else
                return _serviceProvider.GetKeyedServices(type, key).First();
        }

        public static IDomainService<T> ResolveDomainService<T>(string key = null) where T : IEntityBase
        {
            if (key is null)
            {
                if (typeof(T).IsImplementationOfClass(typeof(PersistentEntity)))
                {
                    return Resolve<IDomainService<T>>("PersistentEntity");
                }
                else
                {
                    return Resolve<IDomainService<T>>("IEntity");
                }
            }
            else
            {
                return Resolve<IDomainService<T>>(key);
            }
        }

        public static object ResolveDomainService(Type type, string key = null)
        {
            var genericType = typeof(IDomainService<>).MakeGenericType(type);
            if (key is null)
            {
                if (type.IsImplementationOfClass(typeof(PersistentEntity)))
                    return Resolve(genericType, "PersistentEntity");
                else
                    return Resolve(genericType, "IEntity");
            }
            else
            {
                return Resolve(genericType, key);
            }
        }

        public static ObjectDomainService ResolveObjectDomainService(Type type)
        {
            return new ObjectDomainService(ResolveDomainService(type));
        }
    }
}
