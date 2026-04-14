using Microsoft.AspNetCore.Mvc;
using TiendaVentas.Web.Helpers;
using TiendaVentas.Web.Models;
using TiendaVentas.Web.Services;

namespace TiendaVentas.Web.Controllers
{
    public class CarritoController : Controller
    {
        private const string CarritoSessionKey = "CARRITO_MC_NAILS";
        private readonly ProductoService _productoService;

        public CarritoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            var vm = new CarritoViewModel
            {
                Items = carrito
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(int idProducto, int cantidad = 1)
        {
            if (cantidad <= 0) cantidad = 1;

            var producto = await _productoService.ObtenerProductoPorIdAsync(idProducto);
            if (producto == null)
                return NotFound();

            var carrito = ObtenerCarrito();

            var itemExistente = carrito.FirstOrDefault(x => x.Id_Producto == idProducto);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    Id_Producto = producto.Id_Producto,
                    Nombre = producto.Nombre,
                    Imagen_Url = producto.Imagen_Url,
                    Precio = producto.Precio,
                    Cantidad = cantidad,
                    Categoria = producto.Categoria
                });
            }

            GuardarCarrito(carrito);

            TempData["Success"] = "Producto agregado al carrito correctamente.";
            return RedirectToAction("Index", "Productos");
        }

        [HttpPost]
        public IActionResult Incrementar(int idProducto)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(x => x.Id_Producto == idProducto);

            if (item != null)
            {
                item.Cantidad++;
                GuardarCarrito(carrito);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Disminuir(int idProducto)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(x => x.Id_Producto == idProducto);

            if (item != null)
            {
                item.Cantidad--;

                if (item.Cantidad <= 0)
                {
                    carrito.Remove(item);
                }

                GuardarCarrito(carrito);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Eliminar(int idProducto)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(x => x.Id_Producto == idProducto);

            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarrito(carrito);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove(CarritoSessionKey);
            return RedirectToAction("Index");
        }

        private List<CarritoItem> ObtenerCarrito()
        {
            return HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey) ?? new List<CarritoItem>();
        }

        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetObject(CarritoSessionKey, carrito);
        }
    }
}