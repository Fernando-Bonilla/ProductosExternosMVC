using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductosExternosMVC.Models;
using ProductosExternosMVC.Services;

namespace ProductosExternosMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ServicioProductos servicioProductos = new ServicioProductos();

            ViewBag.Productos = await servicioProductos.Todos();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(CrearProductoDto _crearProductoDto)
        {
            // Validaciones varias
            // ....

            ServicioProductos servicioProductos = new ServicioProductos();
            ProductoDto? producto = await servicioProductos.CrearProducto(_crearProductoDto.nombre, _crearProductoDto.precio);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BorrarProducto(string id)
        {
            Console.WriteLine($"borrar id: {id}");
            //id = id.ToString();
            ServicioProductos servicioProductos = new ServicioProductos();

            servicioProductos.Borrar(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPorKeyword(string keyword)
        {
            Console.WriteLine($" desde mvc {keyword}");
            ServicioProductos servicio = new ServicioProductos();
            List<ProductoDto> productos = await servicio.BuscarPorKeyword(keyword.ToLower());

            ViewBag.Productos = productos;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarProducto(int precioMin, int precioMax)
        {
            Console.WriteLine("Filtrar proddddd mvc");
            ServicioProductos servicio = new ServicioProductos();
            List<ProductoDto> productos = await servicio.FiltrarProductos(precioMin, precioMax);

            ViewBag.Productos = productos;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
