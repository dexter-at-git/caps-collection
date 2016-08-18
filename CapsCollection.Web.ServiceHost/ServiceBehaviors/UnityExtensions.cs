using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Practices.Unity;

namespace CapsCollection.Web.ServiceHost.ServiceBehaviors
{
    public static class UnityExtensions
    {
        private static void RegisterAutoMapperProfiles(IUnityContainer container)
        {
            IEnumerable<Type> autoMapperProfileTypes = AllClasses.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .Where(type => type != typeof(Profile) && typeof(Profile).IsAssignableFrom(type));

            autoMapperProfileTypes.Each(autoMapperProfileType =>
                container.RegisterType(typeof(Profile),
                    autoMapperProfileType,
                    autoMapperProfileType.FullName,
                    new ContainerControlledLifetimeManager(),
                    new InjectionMember[0]));
        }

        /*
        public static void RegisterAutoMapperType(this IUnityContainer container, LifetimeManager lifetimeManager = null)
        {
            RegisterAutoMapperProfiles(container);

            var profiles = container.ResolveAll<Profile>();
            var autoMapperConfigurationStore = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            profiles.Each(autoMapperConfigurationStore.AddProfile);

            autoMapperConfigurationStore.AssertConfigurationIsValid();

            container.RegisterInstance<IConfigurationProvider>(autoMapperConfigurationStore, new ContainerControlledLifetimeManager());
            container.RegisterInstance<IConfiguration>(autoMapperConfigurationStore, new ContainerControlledLifetimeManager());

            container.RegisterType<IMappingEngine, MappingEngine>(lifetimeManager ?? new TransientLifetimeManager(), new InjectionConstructor(typeof(IConfigurationProvider)));
        }
        */
    }
}