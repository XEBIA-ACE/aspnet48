// b41c16c1-0016-5b8d-8f70-29e608d4c11f / FR-S1-011 / T011: ASP.NET Core copy of ../../Controllers/CustomersController.cs.
// Assumption: the legacy controller depends on System.Web.Http and cannot be linked unchanged, so it is
// duplicated with compile-time-only edits (using, ApiController->ControllerBase, IHttpActionResult->IActionResult,
// RoutePrefix->Route). Action logic is unchanged. Not mapped as endpoints until the routing story.
using MicroProjectApplication.Models;
using MicroProjectApplication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MicroProjectApplication.Controllers
{
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            return Ok(_customerService.GetAll());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            var customer = _customerService.GetById(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Create(Customer customer)
        {
            var created = _customerService.Create(customer);
            if (created == null)
                return BadRequest("Invalid customer.");

            return Ok(created);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Update(int id, Customer customer)
        {
            var updated = _customerService.Update(id, customer);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _customerService.Delete(id);
            if (!deleted)
                return NotFound();

            return Ok("Customer deleted successfully.");
        }
    }
}
