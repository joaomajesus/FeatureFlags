using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestHeadersFeatureDefinitionProvider(this IServiceCollection services, Action<RequestHeadersFeatureDefinitionProviderOptions> configureOptions)
        {
            var service = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IFeatureDefinitionProvider))
                ?? throw new InvalidOperationException("Could not find any IFeatureDefinitionProvider service registered. Ensure that \"AddRequestHeadersFeatureDefinitionProvider()\" is called after \"AddFeatureManagement()\"");

            services.Remove(service);

            services.AddSingleton(service.ImplementationType);
            
            var newServiceType = typeof(RequestHeadersFeatureDefinitionProviderDecorator<>).MakeGenericType(service.ImplementationType);
                
            services.Add(ServiceDescriptor.Singleton(typeof(IFeatureDefinitionProvider), newServiceType));

            services.Configure(configureOptions);

            return services;
        }
        
        public static IServiceCollection AddRequestHeadersFeatureFilter(this IServiceCollection services)
        {
            services.TryAddSingleton<RequestHeadersFeatureFilter>();
            
            return services;
        }
    }
}