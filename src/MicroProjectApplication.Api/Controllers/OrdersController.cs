// b41c16c1-0016-5b8d-8f70-29e608d4c11f / FR-S1-011 / T011: ASP.NET Core copy of ../../Controllers/OrdersController.cs.
// Assumption: the legacy controller depends on System.Web.Http and cannot be linked unchanged, so it is
// duplicated with compile-time-only edits (using, ApiController->ControllerBase, IHttpActionResult->IActionResult,
// RoutePrefix->Route). Action logic is unchanged. Not mapped as endpoints until the routing story.
using MicroProjectApplication.Models;
using MicroProjectApplication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MicroProjectApplication.Controllers
{
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            return Ok(_orderService.GetAll());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Create(Order order)
        {
            var created = _orderService.Create(order);
            if (created == null)
                return BadRequest("Invalid order. Check CustomerId, ProductId, and Quantity.");

            return Ok(created);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _orderService.Delete(id);
            if (!deleted)
                return NotFound();

            return Ok("Order deleted successfully.");
        }
    }
}
