using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoService _productoService;
        private readonly CategoriaService _categoriaService;

        public ProductosController(ProductoService productoService, CategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index(int? idCategoria)
        {
            var productos = await _productoService.ObtenerProductosAsync(idCategoria);
            var categorias = await _categoriaService.ObtenerCategoriasAsync();

            ViewBag.Categorias = categorias;
            ViewBag.IdCategoria = idCategoria;

            return View(productos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }
    }
}