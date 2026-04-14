using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Helpers;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.ViewComponents
{
    public class CarritoResumenViewComponent : ViewComponent
    {
        private const string CarritoSessionKey = "CARRITO_MC_NAILS";

        public IViewComponentResult Invoke()
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey) ?? new List<CarritoItem>();
            var totalItems = carrito.Sum(x => x.Cantidad);

            return View(totalItems);
        }
    }
}