using Microsoft.Extensions.Configuration;
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
            services.AddHttpClient("BookOfHabitsServiceClient", c => c.BaseAddress = new Uri(apiGatewayConnection.BookOfHabitsServiceBaseUrl));
            services.AddHttpClient("DiaryServiceBaseUrlClient", c => c.BaseAddress = new Uri(apiGatewayConnection.DiaryServiceBaseUrl));
            services.AddHttpClient("MagazineServiceBaseUrlClient", c => c.BaseAddress = new Uri(apiGatewayConnection.MagazineServiceBaseUrl));

            services.Configure<ApiGateWaySettings>(options =>
            {
                options.BookOfHabitsServiceBaseUrl = apiGatewayConnection.BookOfHabitsServiceBaseUrl;
                options.DiaryServiceBaseUrl = apiGatewayConnection.DiaryServiceBaseUrl;
                options.MagazineServiceBaseUrl = apiGatewayConnection.MagazineServiceBaseUrl;
                options.RoomDesignerServiceBaseUrl = apiGatewayConnection.RoomDesignerServiceBaseUrl;
                options.ValidApiKeys = apiGatewayConnection.ValidApiKeys;
            });
            return services;
        }
    }
}
