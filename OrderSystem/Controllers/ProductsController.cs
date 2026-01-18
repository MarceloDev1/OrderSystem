using Microsoft.AspNetCore.Mvc;

namespace OrderSystem.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // MVP: lista fixa (depois você liga no banco)
        var products = new[]
        {
            new { id = 1, name = "Coca-Cola 350ml" },
            new { id = 2, name = "Pão de queijo" },
            new { id = 3, name = "Hambúrguer" },
        };

        return Ok(products);
    }
}
