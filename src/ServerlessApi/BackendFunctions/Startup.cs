using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerlessApi.Context;

[assembly: FunctionsStartup(typeof(BackendFunctions.Startup))]

namespace BackendFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<FunctionOptions>()
                .Configure<IConfiguration>((settings, configuration) => 
                {
                    configuration.Bind(settings);
                });
            builder.Services.AddDbContext<MonitoringDbContext>(opt => opt.UseSqlServer("Server=tcp:oleskiithesis.database.windows.net,1433;Initial Catalog=ThesisDb;Persist Security Info=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
        }
    }
}
