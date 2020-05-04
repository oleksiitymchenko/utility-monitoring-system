using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ServerlessApi.Context
{
    public class MonitoringDbContext : DbContext
    {
        public IConfiguration Configuration { get; }

        public MonitoringDbContext(DbContextOptions<MonitoringDbContext>  opt):base(opt)
        {
        }

        public virtual DbSet<TelemetryRecord> TelemetryRecord { get; set; }
        public virtual DbSet<ControllerRegistry> ControllerRegistry { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Server=tcp:oleskiithesis.database.windows.net,1433;Initial Catalog=ThesisDb;Persist Security Info=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            connection.AccessToken = GetToken().Result;
            optionsBuilder.UseSqlServer(connection);
        }

        private async Task<string> GetToken()
        {
            AzureServiceTokenProvider provider = new AzureServiceTokenProvider();
            var token = await provider.GetAccessTokenAsync("https://database.windows.net");
            return token;
        }
    }
}
