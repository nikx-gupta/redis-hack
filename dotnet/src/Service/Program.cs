using DevIgnite.Services.Shared;
using Microsoft.Extensions.Hosting;

namespace Hack.Service.RedisDb {
    public class Program {
        public static void Main(string[] args) {
            Host.CreateDefaultBuilder()
                .EnableMicroservice<Startup, CurrentServiceConfiguration>().Build().Run();
        }
    }
}