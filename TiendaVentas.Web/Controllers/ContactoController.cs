using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ContactoImagenService _contactoImagenService;

        public ContactoController(ContactoImagenService contactoImagenService)
        {
            _contactoImagenService = contactoImagenService;
        }

        public async Task<IActionResult> Index()
        {
            var imagenes = await _contactoImagenService.ObtenerActivasAsync();
            return View(imagenes);
        }
    }
}