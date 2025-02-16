﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebAPI.Settings;

namespace WebAPI
{
    public static class Registrar
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var apiGatewayConnection = configuration.Get<ApplicationSettings>().ApiGateWaySettings;
            services.AddSingleton(apiGatewayConnection);
            services.AddHttpClient("RoomDesignerServiceClient", c => c.BaseAddress = new Uri(apiGatewayConnection.RoomDesignerServiceBaseUrl));
            services.Configure<ApiGateWaySettings>(options =>
            {
                options.RoomDesignerServiceBaseUrl = apiGatewayConnection.RoomDesignerServiceBaseUrl;
                options.ValidApiKeys = apiGatewayConnection.ValidApiKeys;
            });
            return services;
        }
    }
}
