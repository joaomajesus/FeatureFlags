# HTTP Request Headers Feature Management Proof of Concept
## Background
...
## Purpose
Implement feature toggling per request, on ASP.NET Core API projects, using HTTP Request Headers, and propagate those Headers to downstream API calls, in a way that:
- Is standartizable across the company.
- Uses the latest .NET Core features.
- Is transparent, unobtrusive and works behind the scenes.
- Has a low entry barrier.
- Requires minimal work to implement.
- Is setup based on configuration.
- Is extensible.
- Is compatible with all the Microsoft Feature Management features and extensions.
## Possible Use Cases
- Enable specific test flows per call.
- Enable mocking calls to external APIs during a specific call.
- Enable or increase the level of detail in logs/telemetry during a specific call to help troubleshooting an issue.
- Enable Dark Production testing.
- Enable and define the behavior of mocks without the need to resort to special identifiers (for ex.: GTIN, PreOrderCode, LaunchId). This way mock configuration and enablement can be simplified and be standartized across projects.
- Release a disabled feature and only enable it for validation purposes before making it generally available. 
- A/B testing.
- Forward Feature Flag Request Headers to downstream APIs to enable the previous use cases E2E.
## Approaches
- Use Feature Management's common extensibility point, Feature Filters, to toggle a feature flag based on request headers.
- Implement a decorator over the default Feature Definition Provider to override the state of the flags.
- Use the Microsoft Header Propagation library to propagate the Feature Flag Request Headers, behind the scenes, for external calls when using the HttpClient.
- Use Azure AppConfiguration as use case of dynamic configuration and an approximation to the future integration with ESP for managing settings and feature flags, to validate and future proof the implementation. 
## Further Improvements
- [ ] Use JWT Scopes to authorize the toggling of a Feature Flag with by a HTTP Header.
- [ ] Cache the resulting state of the Feature Flag for a specific request (the feature flag could be used more than once throughout the application).
## Open Questions
-
## Pros & Cons
### Feature Filter
#### Pros
- 
#### Cons
- 
### Feature Definition Provider Decorator
#### Pros
- 
#### Cons
- 
## Topology
Client --> API1 --> API2

TBD: add diagrams
## Setup
...
## Projects
- TestFeatureFlags.Api1 ...
  - testCalls.http - [Rest Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) for VSCode file with test calls to API1 and header propagation to API2.
- TestFeatureFlags.Api2 ...
  - testCalls.http - [Rest Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) for VSCode file with test calls to API2.
- FeatureManagement ...
  - RequestHeadersFeatureDefinitionProviderDecorator.cs ...
  - RequestHeadersFeatureDefinitionProviderOptions.cs ...
  - RequestHeadersFeatureFilter.cs ...
  - ServiceCollectionExtensions.cs ...
## NuGets:
- [Microsoft.FeatureManagement.AspNetCore](https://www.nuget.org/packages/Microsoft.FeatureManagement.AspNetCore)
- [Microsoft.AspNetCore.HeaderPropagation](https://www.nuget.org/packages/Microsoft.AspNetCore.HeaderPropagation)
## Reference Material
...
