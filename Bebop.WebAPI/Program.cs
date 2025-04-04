﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI;
using WebAPI.Services;

namespace ClientApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors();
            builder.Services.AddHttpClients(builder.Configuration);

            // adds DI services to DI and configures bearer as the default scheme
            //builder.Services.AddAuthentication("Bearer")
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer("Bearer", options =>
                {
                    // identity server issuing token
                    options.Authority = "https://identityserver:8081"; 
                    options.RequireHttpsMetadata = true;

                    // allow self-signed SSL certs
                    options.BackchannelHttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } };

                    // the scope id of this api
                    options.Audience = "doughnutapi";
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Логируйте ошибку аутентификации
                            Console.WriteLine("Authentication failed: " + context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notification"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddSignalR();

            builder.Services.AddMvcCore(options =>
            {
                options.EnableEndpointRouting = false;
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();

           

            IdentityModelEventSource.ShowPII = true;

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCors(builder =>
               builder
                 .WithOrigins("http://localhost:3000")
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials()
             );

            app.UseRouting();
          
            // adds authentication middleware to the pipeline so authentication will be performed on every request
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc();
            app.MapHub<NotificationHub>("/hubs/notification");



            app.Run();
            //CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.ListenAnyIP(5001, listenOptions =>
                    {
                        listenOptions.UseHttps("./certs/identity.pfx", "Pass@habitapi");
                    });
                });
    }
}
