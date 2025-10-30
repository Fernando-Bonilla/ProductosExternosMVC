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
            _apiUrl = "https://68ffe1e9e02b16d1753f8cfe.mockapi.io/api/v1/productos";
            _httpClient = new HttpClient();
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
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
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

            HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, content);

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
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

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
