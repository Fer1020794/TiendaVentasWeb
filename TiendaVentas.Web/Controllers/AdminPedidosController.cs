using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class AdminPedidosController : Controller
    {
        private readonly PedidoService _pedidoService;

        public AdminPedidosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var pedidos = await _pedidoService.ObtenerPedidosAsync();
            return View(pedidos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var pedido = await _pedidoService.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
                return NotFound();

            var detalle = await _pedidoService.ObtenerDetallePedidoAsync(id);

            ViewBag.Pedido = pedido;
            return View(detalle);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var pedido = await _pedidoService.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
                return NotFound();

            if (pedido.Estado != "GENERADO")
            {
                TempData["Success"] = "Solo se pueden confirmar pedidos en estado GENERADO.";
                return RedirectToAction("Detalle", new { id });
            }

            await _pedidoService.ActualizarEstadoAsync(id, "CONFIRMADO");
            TempData["Success"] = "Pedido confirmado correctamente.";

            return RedirectToAction("Detalle", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Cancelar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var pedido = await _pedidoService.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
                return NotFound();

            if (pedido.Estado == "ENTREGADO")
            {
                TempData["Success"] = "No se puede cancelar un pedido ya entregado.";
                return RedirectToAction("Detalle", new { id });
            }

            await _pedidoService.ActualizarEstadoAsync(id, "CANCELADO");
            TempData["Success"] = "Pedido cancelado correctamente.";

            return RedirectToAction("Detalle", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Entregar(int id)
        {
            if (HttpContext.Session.GetString("ADMIN_LOGUEADO") != "SI")
                return RedirectToAction("Login", "AdminAuth");

            var pedido = await _pedidoService.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
                return NotFound();

            if (pedido.Estado != "CONFIRMADO")
            {
                TempData["Success"] = "Solo se pueden marcar como entregados los pedidos CONFIRMADOS.";
                return RedirectToAction("Detalle", new { id });
            }

            await _pedidoService.ActualizarEstadoAsync(id, "ENTREGADO");
            TempData["Success"] = "Pedido marcado como entregado.";

            return RedirectToAction("Detalle", new { id });
        }
    }
}