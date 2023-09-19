using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.FeatureManagement;

namespace FeatureManagement
{
    /// <summary>
    /// A feature definition provider that decorates an IFilterDefinitionProvider and overwrites the flag state based on Request Headers.
    /// </summary>
    internal sealed class RequestHeadersFeatureDefinitionProviderDecorator<T> : IFeatureDefinitionProvider
        where T : IFeatureDefinitionProvider
    {
        private readonly T _featureDefinitionProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<RequestHeadersFeatureDefinitionProviderOptions> _options;

        public RequestHeadersFeatureDefinitionProviderDecorator(T featureDefinitionProvider,
                                                                IHttpContextAccessor httpContextAccessor,
                                                                IOptions<RequestHeadersFeatureDefinitionProviderOptions> options)
        {
            _featureDefinitionProvider = featureDefinitionProvider ?? throw new ArgumentNullException(nameof(featureDefinitionProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
        {
            if (featureName == null)
                throw new ArgumentNullException(nameof(featureName));

            return ApplyRequestHeaders(await _featureDefinitionProvider.GetFeatureDefinitionAsync(featureName));
        }

        public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
        {
            await foreach (var definition in _featureDefinitionProvider.GetAllFeatureDefinitionsAsync())
                yield return ApplyRequestHeaders(definition);
        }

        private FeatureDefinition ApplyRequestHeaders(FeatureDefinition featureDefinition)
        {
            StringValues stringValues = default;
            bool? enable = null;

            var headers = _httpContextAccessor.HttpContext
                                              ?.Request
                                              .Headers;

            var headerSuffixes = _options.Value.HeaderSuffixes.Any()
                                     ? _options.Value.HeaderSuffixes
                                     : new List<string> { string.Empty };

            foreach (var key in headerSuffixes.Select(suffix => $"{suffix}{featureDefinition.Name}"))
            {
                enable = headers?.TryGetValue(key, out stringValues) == true
                         || headers?.TryGetValue(key.ToLower(), out stringValues) == true
                             ? bool.TryParse(stringValues.FirstOrDefault(), out var boolValue)
                               && boolValue
                             : (bool?)null;

                if (enable == true)
                    break;
            }

            if (!enable.HasValue)
                return featureDefinition;

            var newFeatureDefinition = new FeatureDefinition { Name = featureDefinition.Name };

            var enabledFor = featureDefinition.EnabledFor
                                              .Select(config => new FeatureFilterConfiguration
                                              {
                                                  Name = config.Name,
                                                  Parameters = config.Parameters
                                              })
                                              .ToList();

            newFeatureDefinition.EnabledFor = !enable.Value && enabledFor.Any()
                                                  ? new List<FeatureFilterConfiguration>()
                                                  : enable.Value && !enabledFor.Any()
                                                      ? new List<FeatureFilterConfiguration> { new() { Name = "AlwaysOn" } }
                                                      : enabledFor;

            return newFeatureDefinition;
        }
    }
}
