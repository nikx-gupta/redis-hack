using System.Net;
using DevIgnite.Services.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Hack.Service.RedisDb
{
    public class CurrentServiceConfiguration : ContainerConfigurationBase
    {
        protected override void InjectServices()
        {
            _services.AddSingleton(sp => ConnectionMultiplexer.Connect(new ConfigurationOptions()
            {
                EndPoints = {new DnsEndPoint(_configuration["RedisConnection"], 6379)}
            }));
        }
    }
}