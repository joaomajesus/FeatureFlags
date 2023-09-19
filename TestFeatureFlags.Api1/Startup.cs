using FeatureManagement.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

namespace TestFeatureFlags.Api1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if(Configuration.GetConnectionString("AppConfig") != null)
                services.AddAzureAppConfiguration();

            services.AddRouting();

            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add("x-flag-featurea");
                options.Headers.Add("x-flag-featureb");
                options.Headers.Add("x-flag-featurec");
                options.Headers.Add("X-Flag-FeatureA");
                options.Headers.Add("X-Flag-FeatureB");
                options.Headers.Add("X-Flag-FeatureC");
            });

            services.AddHttpClient("HeaderPropagation")
                    .AddHeaderPropagation();
            
            services.AddHttpContextAccessor();

            services.AddControllers();

            services.AddRequestHeadersFeatureFilter();

            services.AddFeatureManagement()
                    .AddRequestHeadersFeatureFilter();

            services.AddRequestHeadersFeatureDefinitionProvider(options => options.HeaderSuffixes.Add("X-Flag-"));
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            if(Configuration.GetConnectionString("AppConfig") != null) 
                app.UseAzureAppConfiguration();

            app.UseHeaderPropagation();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
         }
    }
}
