using System;
using System.Linq;
using dm.lib.infrastructure.nuget;
using dm.lib.exceptionmanager.nuget;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using enigmas.ConfigurableUI.API;

namespace enigmas.ConfigurableUI.API
{

    public class Startup : GepStartup
    {
        public const string APPLICATION_ID = "67";
        public const string APPLICATION_NAME = "ConfigurableUI";
        public const string API_VERSION = "1";
        public static string IngressRouteName = string.Empty;
        public static string connection = "";
        public static string container = "";

        public Startup(IConfiguration configuration) : base(configuration, APPLICATION_ID)
        {
            Startup.IngressRouteName = GetLocalConfiguration("IngressRouteName");

            //Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            this.ConfigureControllers(services);
            base.ConfigureServices(services);
            RegisterDataAccess.Register(services);
            RegisterBusinessService.Register(services);
            RegisterSwagger.Register(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app,
                                IWebHostEnvironment env,
                                IServiceProvider service,
                                IHostApplicationLifetime applicationLifetime)
        {
            RegisterSwagger.UseSwagger(app, false);
            base.Configure(app, env, service, applicationLifetime);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureControllers(IServiceCollection Services)
        {
            // find all controllers
            var Controllers =
                from a in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(ControllerAttribute), true)
                where attributes?.Length > 0
                select new { Type = t };

            var ControllersList = Controllers.ToList();
            Console.WriteLine($"Found {ControllersList.Count} controllers");

            // register them
            foreach (var Controller in ControllersList)
            {
                Console.WriteLine($"[Controller] Registering {Controller.Type.Name}");
                Services
                    .AddMvc()
                    .AddNewtonsoftJson(Options => Options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                    .AddApplicationPart(Controller.Type.Assembly);
            }
        }

        private string GetLocalConfiguration(string key)
        {
            string rt = string.Empty;
            try
            {
                Console.WriteLine("GetLocalConfiguration KEY:" + key);
                rt = Configuration[GepConstants.APP_SETTINGS_PREFIX + key];
                Startup.connection = Configuration[GepConstants.APP_SETTINGS_PREFIX + "DataStoreConnection"];
                Startup.container = Configuration[GepConstants.APP_SETTINGS_PREFIX + "Container"];
                Console.WriteLine("GetLocalConfiguration VALUE:" + rt);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLocalConfiguration ERROR:" + ex.ToString());
            }

            return rt;
        }
    }
}
