using System;
using System.Configuration;
using System.Net;
using C3.ServiceFabric.HttpCommunication;
using C3.ServiceFabric.HttpServiceGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChocolateGateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<HttpCommunicationOptions>(c =>
            {
                c.MaxRetryCount = 8;
                c.OperationTimeout = TimeSpan.FromSeconds(15);
            });
            services.AddServiceFabricHttpCommunication();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/orders", appBuilder =>
            {
                appBuilder.RunHttpServiceGateway(new HttpServiceGatewayOptions
                {
                    ServiceName = new Uri("fabric:/Microservices.ServiceFabric/Front"),
                });
            });

            // catch-all
            app.Run(async context =>
            {
                var logger = loggerFactory.CreateLogger("Catch-All");
                logger.LogWarning("No endpoint found for request {path}", context.Request.Path + context.Request.QueryString);

                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("Not Found");
            });
        }
    }
}
