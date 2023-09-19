using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.FeatureManagement;

namespace FeatureManagement
{
    [FilterAlias("RequestHeaders")]
    internal class RequestHeadersFeatureFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestHeadersFeatureFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var parameters = context.Parameters.Get<RequestHeadersFeatureFilterParameters>();

            if (parameters == null)
                return Task.FromResult(true);

            var key = parameters.RequestHeader;
            
            if(string.IsNullOrEmpty(key))
                return Task.FromResult(true);

            StringValues stringValues = default;

            var headers = _httpContextAccessor.HttpContext
                                              ?.Request
                                              .Headers;

            return Task.FromResult(headers?.TryGetValue(key.ToLower(), out stringValues) != true 
                                   && headers?.TryGetValue(key,        out stringValues) != true 
                                   || bool.TryParse(stringValues.FirstOrDefault(), out var boolValue) 
                                   && boolValue);
        }

        private class RequestHeadersFeatureFilterParameters
        {
            public string              RequestHeader { get; set; }
            public IEnumerable<string> Scopes        { get; set; } //TBD
        }
    }
}