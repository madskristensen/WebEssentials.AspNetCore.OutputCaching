using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebEssentials.AspNetCore.OutputCaching;
using WebMarkupMin.AspNetCore2;

namespace Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOutputCaching(options =>
            {
                options.Profiles["default"] = new OutputCacheProfile
                {
                    Duration = 600,
                    FileDependencies= new []{ ""}
                };
            });

            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
                options.DisablePoweredByHttpHeaders = true;
            }).AddHtmlMinification(options =>
            {
                options.MinificationSettings.RemoveOptionalEndTags = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();

            app.UseOutputCaching();
            app.UseWebMarkupMin();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
