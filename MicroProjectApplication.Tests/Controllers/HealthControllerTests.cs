using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MicroProjectApplication.Tests.Controllers
{
    public class HealthControllerTests
    {
        [Fact]
        public async Task GetHealth_ReturnsOkWithStatusOk()
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            config.EnsureInitialized();

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var response = await client.GetAsync("http://localhost/health");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

                var body = JObject.Parse(await response.Content.ReadAsStringAsync());
                Assert.Equal("ok", (string)body["status"]);
            }
        }
    }
}
