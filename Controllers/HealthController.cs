using System.Web.Http;

namespace MicroProjectApplication.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok("Service is running.");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("~/health")]
        public IHttpActionResult GetStatus()
        {
            return Ok(new { status = "ok" });
        }
    }
}