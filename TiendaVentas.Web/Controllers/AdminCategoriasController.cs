using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class AdminCategoriasController : Controller
    {
        private readonly CategoriaService _categoriaService;

        public AdminCategoriasController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var categorias = await _categoriaService.ObtenerTodasAdminAsync();
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            return View(new Categoria { Estado = "A" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Categoria model)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            if (!ModelState.IsValid)
                return View(model);

            await _categoriaService.CrearAsync(model);
            TempData["Success"] = "Categoría creada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Categoria model)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            if (!ModelState.IsValid)
                return View(model);

            await _categoriaService.ActualizarAsync(model);
            TempData["Success"] = "Categoría actualizada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            await _categoriaService.BajaLogicaAsync(id);
            TempData["Success"] = "Categoría desactivada correctamente.";
            return RedirectToAction("Index");
        }
    }
}