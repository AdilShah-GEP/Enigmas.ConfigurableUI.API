using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using dm.lib.core.nuget;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace enigmas.ConfigurableUI.API
{
    public static class RegisterSwagger
    {
        private const string INTERNAL_API_VERSION = "v" + Startup.API_VERSION;
        private const string CUSTOMER_API_VERSION = "c" + Startup.API_VERSION;
        private const string INTERNAL_APP_NAME = Startup.APPLICATION_NAME;
        private const string CUSTOMER_APP_NAME = Startup.APPLICATION_NAME + "_Customer";
        private const string INTERNAL_SWAGGER_URL = "/swagger/" + INTERNAL_API_VERSION + "/swagger.json";
        private const string CUSTOMER_SWAGGER_URL = "/swagger/" + CUSTOMER_API_VERSION + "/swagger.json";
        /// <summary>
        /// This will appear as api description. swagger.json->info.description
        /// </summary>
        private const string INTERNAL_API_DESCRIPTION = "An endpoint to access a group of apis";
        private const string CUSTOMER_API_DESCRIPTION = "An endpoint to access a group of apis";

        /// <summary>
        /// A link where a user can go read the documents realted to API
        /// </summary>
        private const string INTERNAL_API_DOCUMENTATION_URL = "https://smartbygep.atlassian.net/";
        private const string CUSTOMER_API_DOCUMENTATION_URL = "https://smartbygep.atlassian.net/";
        private const string API_TEAM_EMAIL = "dm.teamabc@gep.com";

        public static void Register(IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(INTERNAL_API_VERSION, new OpenApiInfo
                {
                    Title = INTERNAL_APP_NAME,
                    Version = INTERNAL_API_VERSION,
                    Description = INTERNAL_API_DESCRIPTION,
                    Contact = new OpenApiContact
                    {
                        Name = "Developer Team",
                        Email = API_TEAM_EMAIL,
                        Url = new Uri(INTERNAL_API_DOCUMENTATION_URL),
                    }
                });
                c.SwaggerDoc(CUSTOMER_API_VERSION, new OpenApiInfo
                {
                    Title = CUSTOMER_APP_NAME,
                    Version = CUSTOMER_API_VERSION,
                    Description = CUSTOMER_API_DESCRIPTION,
                    Contact = new OpenApiContact
                    {
                        Name = "API Documentation/Wiki",
                        Url = new Uri(CUSTOMER_API_DOCUMENTATION_URL),
                    }
                });

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (docName == INTERNAL_API_VERSION) return true;
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var classVisible = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<VisibleToCustomerAttribute>();
                    if (classVisible.Any()) return true;

                    var methodVisible = methodInfo
                        .GetCustomAttributes(true)
                        .OfType<VisibleToCustomerAttribute>();

                    return methodVisible.Any();
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();

            });
        }

        public static void UseSwagger(IApplicationBuilder app, bool ShowOnlyCustomeAPIonUI)
        {

            //if (string.IsNullOrEmpty(Startup.IngressRouteName))
            //{
            //    app.UseSwagger();
            //}
            //else
            //{
            //    app.UseSwagger(c =>
            //        c.ToString()
            //        //c.
            //        //c.PreSerializeFilters.Add((swagger, httpReq) =>
            //        //{
            //        //    var oldPaths = swagger.Paths.ToDictionary(entry => entry.Key,
            //        //                                   entry => entry.Value);
            //        //    foreach (var path in oldPaths)
            //        //    {
            //        //        swagger.Paths.Remove(path.Key);
            //        //        swagger.Paths.Add(@"/" + Startup.IngressRouteName + path.Key, path.Value);
            //        //    }
            //        //});
            //       // c.
            //    );
            //}

            if (string.IsNullOrEmpty(Startup.IngressRouteName))
            {
                app.UseSwagger();
            }
            else
            {
                var basePath = @"/" + Startup.IngressRouteName;
                //check
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        //swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = basePath } };
                        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}" } };

                    });
                });

            }

            app.UseSwaggerUI(c =>
            {
                if (!ShowOnlyCustomeAPIonUI)
                {
                    if (string.IsNullOrEmpty(Startup.IngressRouteName))
                    {
                        c.SwaggerEndpoint(INTERNAL_SWAGGER_URL, INTERNAL_APP_NAME);
                    }
                    else
                    {
                        c.SwaggerEndpoint(@"/" + Startup.IngressRouteName + INTERNAL_SWAGGER_URL, INTERNAL_APP_NAME);
                    }
                }
                if (string.IsNullOrEmpty(Startup.IngressRouteName))
                {
                    c.SwaggerEndpoint(CUSTOMER_SWAGGER_URL, CUSTOMER_APP_NAME);
                }
                else
                {
                    c.SwaggerEndpoint(@"/" + Startup.IngressRouteName + CUSTOMER_SWAGGER_URL, CUSTOMER_APP_NAME);
                }
                if (typeof(RegisterSwagger).GetTypeInfo().Assembly.GetManifestResourceNames().Length > 0)
                {
                    var resouceName = typeof(RegisterSwagger).GetTypeInfo().Assembly.GetManifestResourceNames()[0];
                    c.IndexStream = () => typeof(RegisterSwagger).GetTypeInfo().Assembly.GetManifestResourceStream(resouceName);
                }
            });
        }
    }
}
