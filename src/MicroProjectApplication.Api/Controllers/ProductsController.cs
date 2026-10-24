// b41c16c1-0016-5b8d-8f70-29e608d4c11f / FR-S1-011 / T011: ASP.NET Core copy of ../../Controllers/ProductsController.cs.
// Assumption: the legacy controller depends on System.Web.Http and cannot be linked unchanged, so it is
// duplicated with compile-time-only edits (using, ApiController->ControllerBase, IHttpActionResult->IActionResult,
// RoutePrefix->Route). Action logic is unchanged. Not mapped as endpoints until the routing story.
using MicroProjectApplication.Models;
using MicroProjectApplication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MicroProjectApplication.Controllers
{
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            return Ok(_productService.GetAll());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Create(Product product)
        {
            var created = _productService.Create(product);
            if (created == null)
                return BadRequest("Invalid product.");

            return Ok(created);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Update(int id, Product product)
        {
            var updated = _productService.Update(id, product);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _productService.Delete(id);
            if (!deleted)
                return NotFound();

            return Ok("Product deleted successfully.");
        }
    }
}
