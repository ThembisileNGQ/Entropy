using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.Web.HAL.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace HAL.Prototype.Api
{
    public class Startup
    {
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddMvcOptions(c => {
                    c.OutputFormatters.RemoveType<JsonOutputFormatter>();
                    c.OutputFormatters.Add(new JsonHalOutputFormatter(
                        new string[]
                        {
                            "application/hal+json",
                            "application/vnd.example.hal+json",
                            "application/vnd.example.hal.v1+json"
                        }
                    ));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Map("/api", api =>
            {
                api.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}");
                });
            });
        }
    }
}