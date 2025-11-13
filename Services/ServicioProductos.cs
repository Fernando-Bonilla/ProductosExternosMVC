using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProductosExternosMVC.Services
{
    public interface IServicioProductos
    {
        Task<ProductoDto> CrearProducto(string nombre, string precio);
        Task BuscarMostrar(string id);
        Task<ProductoDto> Buscar(string id);
        Task Borrar(string id);
        Task Modificar(ProductoDto productoDto);
        Task<List<ProductoDto>> Todos();

        Task<List<ProductoDto>> BuscarPorKeyword(string keyword);

        Task<List<ProductoDto>> FiltrarProductos(int precioMin, int precioMax);
    }

    // Creamos un Dto para mapear los productos, es necesario para serializar y deserializar
    // La respuesta JSON
    public class ProductoDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("precio")]
        public string? Precio { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }        
        public string? UpdatedAt { get; set; }
    }

    public class ServicioProductos : IServicioProductos
    {
        public readonly string _apiUrl;
        public readonly HttpClient _httpClient;

        public ServicioProductos()
        {
            //_apiUrl = "https://68ffe1e9e02b16d1753f8cfe.mockapi.io/api/v1/productos";
            _apiUrl = "https://localhost:7016/Producto";
            _httpClient = new HttpClient();
        }

        public async Task<List<ProductoDto>> FiltrarProductos(int precioMin, int precioMax)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}/filtrar-por-precio/{precioMin}/{precioMax}");

            if (response.IsSuccessStatusCode) 
            {
                string json = await response.Content.ReadAsStringAsync();
                List<ProductoDto>? productos = JsonSerializer.Deserialize<List<ProductoDto>>(json);

                return productos!;

            }else
            {
                Console.WriteLine($"Error al filtrar los productos{response.StatusCode}");
            }
            return new List<ProductoDto>();
        }

        public async Task<List<ProductoDto>> BuscarPorKeyword(string keyword)
        {
            keyword = keyword.Trim().ToLower();

            HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}/buscar-por-nombre/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<ProductoDto>? prod = JsonSerializer.Deserialize<List<ProductoDto>>(json);

                return prod!;
            }
            else
            {
                Console.WriteLine($"Error al buscar el producto: {response.StatusCode}");
                return new List<ProductoDto>();
            }
        }

        public async Task<ProductoDto> Buscar(string id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ProductoDto? producto = JsonSerializer.Deserialize<ProductoDto>(json);
                return producto!;
            }
            else
            {
                Console.WriteLine($"Error al buscar el producto: {response.StatusCode}");
                return null!;
            }
        }

        public async Task Modificar(ProductoDto productoDto)
        {
            string json = JsonSerializer.Serialize(productoDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"{_apiUrl}/{productoDto.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Producto con ID {productoDto.Id} modificado correctamente.");
            }
            else
            {
                Console.WriteLine($"Error al modificar el producto: {response.StatusCode}");
            }
        }

        public async Task Borrar(string id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}"); //  $"{_apiUrl}/producto?id={id}"


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Producto con ID {id} eliminado correctamente.");
            }
            else
            {
                Console.WriteLine($"Error al eliminar el producto: {response.StatusCode}");
            }
        }

        public async Task BuscarMostrar(string id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ProductoDto? producto = JsonSerializer.Deserialize<ProductoDto>(json);
                Console.WriteLine($"Producto encontrado:");
                Console.WriteLine($"ID: {producto!.Id}");
                Console.WriteLine($"Nombre: {producto.Nombre}");
                Console.WriteLine($"Precio: {producto.Precio}");
                Console.WriteLine($"Fecha de Creación: {producto.CreatedAt}");
            }
            else
            {
                Console.WriteLine($"Error al buscar el producto: {response.StatusCode}");
            }
        }
        public async Task<ProductoDto?> CrearProducto(string nombre, string precio)
        {
            ProductoDto nuevoProducto = new ProductoDto
            {
                Nombre = nombre,
                Precio = precio,
                CreatedAt = DateTime.UtcNow.ToString("o")
            };

            string json = JsonSerializer.Serialize(nuevoProducto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl+"/Crear", content);

            if (response.IsSuccessStatusCode)
            {
                string respuestaJson = await response.Content.ReadAsStringAsync();
                ProductoDto productoCreado = JsonSerializer.Deserialize<ProductoDto>(respuestaJson)!;

                return productoCreado;
            }
            else
            {
                Console.WriteLine($"Error al crear el producto: {response.StatusCode}");
                return null;
            }
        }
        public async Task<List<ProductoDto>> Todos()
        {
            string endpointApi = "/ListarProductos";
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl+$"{endpointApi}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<ProductoDto> productos = JsonSerializer.Deserialize<List<ProductoDto>>(json);
                return productos;
            }
            else
            {
                return new List<ProductoDto>();
            }
        }
    }
}
