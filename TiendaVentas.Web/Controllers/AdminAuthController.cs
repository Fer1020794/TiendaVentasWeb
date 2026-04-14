using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly AdminAuthService _authService;

        public AdminAuthController(AdminAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") == "SI")
                return RedirectToAction("Index", "AdminProductos");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _authService.LoginAsync(model.Correo, model.Clave);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View(model);
            }

            HttpContext.Session.SetString("ADMIN_LOGUEADO", "SI");
            HttpContext.Session.SetString("ADMIN_NOMBRE", usuario.Nombre_Usuario);
            HttpContext.Session.SetString("ADMIN_CORREO", usuario.Correo);
            HttpContext.Session.SetString("ADMIN_ROL", usuario.Rol);

            return RedirectToAction("Index", "AdminProductos");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("ADMIN_LOGUEADO");
            HttpContext.Session.Remove("ADMIN_NOMBRE");
            HttpContext.Session.Remove("ADMIN_CORREO");
            HttpContext.Session.Remove("ADMIN_ROL");

            return RedirectToAction("Login");
        }
    }
}