using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly CategoriaService _categoriaService;
        private readonly ProductoService _productoService;
        private readonly BannerService _bannerService;

        public HomeController(
            CategoriaService categoriaService,
            ProductoService productoService,
            BannerService bannerService)
        {
            _categoriaService = categoriaService;
            _productoService = productoService;
            _bannerService = bannerService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                Categorias = await _categoriaService.ObtenerCategoriasAsync(),
                ProductosDestacados = await _productoService.ObtenerProductosDestacadosAsync(),
                Banners = await _bannerService.ObtenerActivosAsync()
            };

            return View(model);
        }
    }
}