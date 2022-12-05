using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Startup
{
    public static class ServiceBusStartup
    {
        public static IServiceCollection ConfigureServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAzureClients(builder =>
            {
                builder.AddServiceBusClient(configuration.GetSection("AzureServiceBusSettings").GetValue<string>("ConnectionString"));
            });

            return services;
        }
    }
}
