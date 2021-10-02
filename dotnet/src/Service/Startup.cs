using AutoMapper;
using DevIgnite.Services.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Hack.Service.RedisDb
{
    public class Startup : ServiceStartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void AddAutoMapper(IMapperConfigurationExpression mapper)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }

        protected override void ConfigureEndpoints(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=index}");
                endpoints.MapControllers();
            });
        }
    }
}