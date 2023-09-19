using Microsoft.FeatureManagement;

namespace FeatureManagement.Extensions
{
    public static class FeatureManagementBuilderExtensions
    {
        public static IFeatureManagementBuilder AddRequestHeadersFeatureFilter(this IFeatureManagementBuilder builder)
        {
            return builder.AddFeatureFilter<RequestHeadersFeatureFilter>();
        }
    }
}