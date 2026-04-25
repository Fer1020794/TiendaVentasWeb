using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class AdminBannersController : Controller
    {
        private readonly BannerService _bannerService;
        private readonly IWebHostEnvironment _env;

        public AdminBannersController(
            BannerService bannerService,
            IWebHostEnvironment env)
        {
            _bannerService = bannerService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var banners = await _bannerService.ObtenerTodosAdminAsync();
            return View(banners);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            return View(new Banner
            {
                Estado = "A",
                Orden = 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Banner model, IFormFile? imagen)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            if (imagen == null || imagen.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar una imagen para el banner.");

            if (!ModelState.IsValid)
                return View(model);

            model.Imagen_Url = await GuardarImagenBannerAsync(imagen!);

            await _bannerService.CrearAsync(model);

            TempData["Success"] = "Banner creado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var banner = await _bannerService.ObtenerPorIdAsync(id);

            if (banner == null)
                return NotFound();

            return View(banner);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Banner model, IFormFile? imagen)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var bannerActual = await _bannerService.ObtenerPorIdAsync(model.Id_Banner);

            if (bannerActual == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            if (imagen != null && imagen.Length > 0)
            {
                model.Imagen_Url = await GuardarImagenBannerAsync(imagen);
            }
            else
            {
                model.Imagen_Url = bannerActual.Imagen_Url;
            }

            await _bannerService.ActualizarAsync(model);

            TempData["Success"] = "Banner actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            await _bannerService.BajaLogicaAsync(id);

            TempData["Success"] = "Banner desactivado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            await _bannerService.ActivarAsync(id);

            TempData["Success"] = "Banner activado correctamente.";
            return RedirectToAction("Index");
        }

        private async Task<string> GuardarImagenBannerAsync(IFormFile imagen)
        {
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
                throw new InvalidOperationException("Solo se permiten imágenes JPG, JPEG, PNG o WEBP.");

            var carpeta = Path.Combine(_env.WebRootPath, "images", "banners");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return $"/images/banners/{nombreArchivo}";
        }
    }
}