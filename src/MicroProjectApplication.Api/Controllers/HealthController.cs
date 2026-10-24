// b41c16c1-0016-5b8d-8f70-29e608d4c11f / FR-S1-011 / T011: ASP.NET Core copy of ../../Controllers/HealthController.cs.
// Assumption: the legacy controller depends on System.Web.Http and cannot be linked unchanged, so it is
// duplicated with compile-time-only edits (using, ApiController->ControllerBase, IHttpActionResult->IActionResult,
// RoutePrefix->Route). Action logic is unchanged. Not mapped as endpoints until the routing story.
using Microsoft.AspNetCore.Mvc;

namespace MicroProjectApplication.Controllers
{
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok("Service is running.");
        }
    }
}
