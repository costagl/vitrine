using Microsoft.AspNetCore.Mvc;

namespace VitrineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
