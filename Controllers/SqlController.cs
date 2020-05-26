using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;

namespace webapi_azure_ad_managed_identity_with_sql.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SqlController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public SqlController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("version")]
        public async Task<IActionResult> GetVersionAsync()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net");

            using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"))
            {
                AccessToken = accessToken
            };
            using var sqlCommand = new SqlCommand("SELECT @@VERSION", sqlConnection);
            await sqlConnection.OpenAsync();
            var version = (string)await sqlCommand.ExecuteScalarAsync();

            return Content(version);
        }
    }
}