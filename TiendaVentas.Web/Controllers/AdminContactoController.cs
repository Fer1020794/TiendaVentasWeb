using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class AdminContactoController : Controller
    {
        private readonly ContactoImagenService _contactoImagenService;
        private readonly IWebHostEnvironment _env;

        public AdminContactoController(
            ContactoImagenService contactoImagenService,
            IWebHostEnvironment env)
        {
            _contactoImagenService = contactoImagenService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var imagenes = await _contactoImagenService.ObtenerTodasAdminAsync();

            return View(imagenes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            return View(new ContactoImagen
            {
                Estado = "A"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ContactoImagen model,
            IFormFile? imagen)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            if (!ModelState.IsValid)
                return View(model);

            if (imagen == null || imagen.Length <= 0)
            {
                ModelState.AddModelError("", "Debe seleccionar una imagen.");
                return View(model);
            }

            model.Imagen_Url = await GuardarImagenAsync(imagen);

            await _contactoImagenService.CrearAsync(model);

            TempData["Success"] = "Imagen de contacto agregada correctamente.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            await _contactoImagenService.DesactivarAsync(id);

            TempData["Success"] = "Imagen desactivada correctamente.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            await _contactoImagenService.ActivarAsync(id);

            TempData["Success"] = "Imagen activada correctamente.";

            return RedirectToAction("Index");
        }

        private async Task<string> GuardarImagenAsync(IFormFile imagen)
        {
            var extensionesPermitidas = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension = Path.GetExtension(imagen.FileName)
                .ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Solo se permiten imágenes JPG, JPEG, PNG o WEBP.");
            }

            var carpeta = Path.Combine(
                _env.WebRootPath,
                "images",
                "contacto");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";

            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return $"/images/contacto/{nombreArchivo}";
        }
    }
}