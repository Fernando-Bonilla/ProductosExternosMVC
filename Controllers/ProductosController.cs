using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductosExternosMVC.Models;
using ProductosExternosMVC.Services;


namespace ProductosExternosMVC.Controllers
{
    public class ProductosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(string id)
        {
            ServicioProductos servicioProductos = new ServicioProductos();

            ProductoDto productoOrigianl = await servicioProductos.Buscar(id);
            EditarProductoDto producto = new EditarProductoDto();
            producto.nombre = productoOrigianl.Nombre;
            producto.precio = productoOrigianl.Precio;
            producto.id = productoOrigianl.Id;

            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(string id, string nombre, string precio)
        {
            Console.WriteLine($"nombre: {nombre}");
            Console.WriteLine($"id: {id}");
            Console.WriteLine($"precio: {precio}");

            ServicioProductos servicioProductos = new ServicioProductos();

            ProductoDto productoDto = new ProductoDto();
            productoDto.Id = id;
            productoDto.Nombre = nombre;
            productoDto.Precio = precio;
            productoDto.CreatedAt = DateTime.UtcNow.ToString("o");
            productoDto.UpdatedAt = DateTime.UtcNow.ToString("o");

            await servicioProductos.Modificar(productoDto);

            return RedirectToAction("Index","Home");
        }
    }
}
