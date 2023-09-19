using System.Collections.Generic;

namespace FeatureManagement
{
    public class RequestHeadersFeatureDefinitionProviderOptions
    {
        public List<string> HeaderSuffixes { get; } = new();
    }
}