using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class AdminProductosController : Controller
    {
        private readonly ProductoService _productoService;
        private readonly CategoriaService _categoriaService;
        private readonly IWebHostEnvironment _env;

        public AdminProductosController(
            ProductoService productoService,
            CategoriaService categoriaService,
            IWebHostEnvironment env)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var productos = await _productoService.ObtenerTodosAdminAsync();
            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            ViewBag.Categorias = await _categoriaService.ObtenerCategoriasAsync();
            return View(new ProductoFormViewModel
            {
                Estado = "A",
                Stock = 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoFormViewModel model)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            ViewBag.Categorias = await _categoriaService.ObtenerCategoriasAsync();

            if (!ModelState.IsValid)
                return View(model);

            string? rutaImagen = null;

            if (model.ImagenFile != null && model.ImagenFile.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images", "productos");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(model.ImagenFile.FileName)}";
                var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaFisica, FileMode.Create);
                await model.ImagenFile.CopyToAsync(stream);

                rutaImagen = $"/images/productos/{nombreArchivo}";
            }

            var producto = new Producto
            {
                Id_Categoria = model.Id_Categoria,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Precio = model.Precio,
                Stock = model.Stock,
                Imagen_Url = rutaImagen,
                Estado = model.Estado
            };

            await _productoService.CrearAsync(producto);
            TempData["Success"] = "Producto creado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
                return NotFound();

            ViewBag.Categorias = await _categoriaService.ObtenerCategoriasAsync();

            var vm = new ProductoFormViewModel
            {
                Id_Producto = producto.Id_Producto,
                Id_Categoria = producto.Id_Categoria,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Imagen_Url = producto.Imagen_Url,
                Estado = producto.Estado
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductoFormViewModel model)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            ViewBag.Categorias = await _categoriaService.ObtenerCategoriasAsync();

            if (!ModelState.IsValid)
                return View(model);

            var rutaImagen = model.Imagen_Url;

            if (model.ImagenFile != null && model.ImagenFile.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images", "productos");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(model.ImagenFile.FileName)}";
                var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaFisica, FileMode.Create);
                await model.ImagenFile.CopyToAsync(stream);

                rutaImagen = $"/images/productos/{nombreArchivo}";
            }

            var producto = new Producto
            {
                Id_Producto = model.Id_Producto,
                Id_Categoria = model.Id_Categoria,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Precio = model.Precio,
                Stock = model.Stock,
                Imagen_Url = rutaImagen,
                Estado = model.Estado
            };

            await _productoService.ActualizarAsync(producto);
            TempData["Success"] = "Producto actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            await _productoService.BajaLogicaAsync(id);
            TempData["Success"] = "Producto desactivado correctamente.";
            return RedirectToAction("Index");
        }
    }
}