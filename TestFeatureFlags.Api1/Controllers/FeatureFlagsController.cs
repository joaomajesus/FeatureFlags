using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace TestFeatureFlags.Api1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeatureFlagsController : ControllerBase
    {
        private readonly IFeatureManager _featureManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public FeatureFlagsController(IFeatureManager featureManager, IHttpClientFactory httpClientFactory)
        {
            _featureManager = featureManager;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("")]
        public Task<IDictionary<string, bool>> Get()
        {
            return GetFeatures();
        }
        
        [HttpGet]
        [Route("WithHeaderPropagation")]
        public async Task<IDictionary<string, IDictionary<string, bool>>> GetWithHeaderPropagation()
        {
            var features = await GetFeatures();

            using var client = _httpClientFactory.CreateClient("HeaderPropagation");

            return await JoinApi2Flags(client, features);
        }
        
        [HttpGet]
        [Route("WithoutHeaderPropagation")]
        public async Task<IDictionary<string, IDictionary<string, bool>>> GetWithoutHeaderPropagation()
        {
            var features = await GetFeatures();

            using var client = _httpClientFactory.CreateClient();

            return await JoinApi2Flags(client, features);
        }

        private async Task<IDictionary<string, bool>> GetFeatures()
        {
            var features = new Dictionary<string, bool>();

            await foreach (var name in _featureManager.GetFeatureNamesAsync())
                features.Add(name, await _featureManager.IsEnabledAsync(name));

            return features;
        }

        private static async Task<IDictionary<string, IDictionary<string, bool>>> JoinApi2Flags(HttpClient client, IDictionary<string, bool> features1)
        {
            using var response = await client.GetAsync(new Uri("http://localhost:18687/FeatureFlags"));

            var features2 = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();

            var features = new Dictionary<string, IDictionary<string, bool>>
            {
                {"Api1", features1},
                {"Api2", features2}
            };

            return features;
        }
    }
}
