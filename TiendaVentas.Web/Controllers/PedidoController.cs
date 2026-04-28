using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Helpers;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class PedidoController : Controller
    {
        private const string CarritoSessionKey = "CARRITO_MC_NAILS";

        private readonly PedidoService _pedidoService;
        private readonly PedidoPdfService _pdfService;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public PedidoController(
            PedidoService pedidoService,
            PedidoPdfService pdfService,
            EmailService emailService,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _pedidoService = pedidoService;
            _pdfService = pdfService;
            _emailService = emailService;
            _env = env;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey) ?? new List<CarritoItem>();

            if (!carrito.Any())
                return RedirectToAction("Index", "Carrito");

            var vm = new PedidoCheckoutViewModel
            {
                Items = carrito
            };

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> Confirmado(int id)
        {
            var pedido = await _pedidoService.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
                return NotFound();

            ViewBag.TelefonoWhatsApp = _configuration["EmpresaSettings:TelefonoWhatsApp"] ?? "50254001440";
            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(PedidoCheckoutViewModel model)
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey) ?? new List<CarritoItem>();

            if (!carrito.Any())
            {
                ModelState.AddModelError("", "El carrito está vacío.");
            }

            model.Items = carrito;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var pedido = new Pedido
                {
                    Nombre_Cliente = model.Nombre_Cliente,
                    Telefono = model.Telefono,
                    Correo_Cliente = model.Correo_Cliente,
                    Direccion = model.Direccion,
                    Departamento = model.Departamento,
                    Municipio = model.Municipio,
                    Observaciones = model.Observaciones,
                    Total = model.Total,
                    Estado = "GENERADO"
                };

                var idPedido = await _pedidoService.CrearPedidoAsync(pedido, carrito);

                var pdfBytes = _pdfService.GenerarPdf(idPedido, model);

                var carpetaPedidos = Path.Combine(_env.WebRootPath, "pedidos");
                if (!Directory.Exists(carpetaPedidos))
                    Directory.CreateDirectory(carpetaPedidos);

                var nombreArchivo = $"Pedido_{idPedido:D6}.pdf";
                var rutaFisica = Path.Combine(carpetaPedidos, nombreArchivo);

                await System.IO.File.WriteAllBytesAsync(rutaFisica, pdfBytes);

                var pdfUrl = $"/pedidos/{nombreArchivo}";
                await _pedidoService.ActualizarPdfUrlAsync(idPedido, pdfUrl);

                try
                {
                    await _emailService.EnviarPedidoAsync(idPedido, rutaFisica);
                    TempData["Success"] = "Pedido generado correctamente y enviado por correo.";
                }
                catch
                {
                    TempData["Success"] = "Pedido generado correctamente, pero no se pudo enviar el correo.";
                }

                HttpContext.Session.Remove(CarritoSessionKey);

                return RedirectToAction("Confirmado", new { id = idPedido });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}