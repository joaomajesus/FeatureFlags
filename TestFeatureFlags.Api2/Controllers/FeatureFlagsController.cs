using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace TestFeatureFlags.Api2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeatureFlagsController : ControllerBase
    {
        private readonly IFeatureManager _featureManager;

        public FeatureFlagsController(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        [HttpGet]
        public async Task<IDictionary<string, bool>> Get()
        {
            var features = new Dictionary<string, bool>();

            await foreach (var name in _featureManager.GetFeatureNamesAsync())
                features.Add(name, await _featureManager.IsEnabledAsync(name));
                
            return features;
        }
    }
}
