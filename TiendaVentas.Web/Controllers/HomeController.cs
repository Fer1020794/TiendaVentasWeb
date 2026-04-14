using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly CategoriaService _categoriaService;
        private readonly ProductoService _productoService;

        public HomeController(CategoriaService categoriaService, ProductoService productoService)
        {
            _categoriaService = categoriaService;
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                Categorias = await _categoriaService.ObtenerCategoriasAsync(),
                ProductosDestacados = await _productoService.ObtenerProductosDestacadosAsync(8)
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}