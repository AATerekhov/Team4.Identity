﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using WebAPI;

namespace ClientApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddHttpClients(Configuration);

            // adds DI services to DI and configures bearer as the default scheme
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    // identity server issuing token
                    options.Authority = "https://identityserver:8081";
                    options.RequireHttpsMetadata = false;

                    // allow self-signed SSL certs
                    options.BackchannelHttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } };

                    // the scope id of this api
                    options.Audience = "doughnutapi";
                });

            services.AddMvcCore(options => {
                options.EnableEndpointRouting = false;
            })
                .AddAuthorization();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IdentityModelEventSource.ShowPII = true;

            app.UseCors(builder =>
                 builder
                   .WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials()
               );
            // adds authentication middleware to the pipeline so authentication will be performed on every request
            app.UseAuthentication();
            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Doughnut API is running!");
            //});
        }
    }
}
