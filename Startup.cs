using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace APIProject
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    // Add framework services.
        //    services.AddApplicationInsightsTelemetry(Configuration);

        //    services.AddMvc();
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string swaggerCommentXmlPath = string.Empty;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                swaggerCommentXmlPath = Environment.CurrentDirectory + "\\bin\\Debug\\net462\\win7-x64\\BASIC.TAP.RestService.xml";
            }
            else
            {
                swaggerCommentXmlPath = Environment.CurrentDirectory + "\\BASIC.TAP.RestService.xml";
            }


            var corsBuilder = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.WithOrigins("http://php.rlogical.com");
            corsBuilder.WithOrigins("http://10.1.0.9:8080");
            corsBuilder.WithOrigins("http://localhost:3000");
            corsBuilder.WithOrigins("http://localhost:5708");
            corsBuilder.AllowCredentials();
            services.AddCors(options => options.AddPolicy("bgk", corsBuilder.Build()));

            // Add framework services.
            services.AddMvc(
                config => config.Filters.Add(typeof(CustomExceptionFilterAttribute))).AddJsonOptions(options =>
                {

                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter
                    {
                        CamelCaseText = true
                    });
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BASIC T&A / Payroll Service", Version = "v1" });
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.IncludeXmlComments(swaggerCommentXmlPath); //Includes XML comment file
            });


        }

    }
}
