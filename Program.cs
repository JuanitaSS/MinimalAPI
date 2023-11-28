using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalAPI.Modelos;

public static class RutasInventario
{
    public static void ConfigurarInventario(WebApplication app)
    {
        List<Producto> inventario = new List<Producto>();

        app.MapGet("/", () => "Bienvenido al inventario");

        app.MapGet("/ObtenerInventario", () => Results.Json(inventario));

        app.MapPost("/AgregarProducto", async (HttpContext context) =>
        {
            try
            {
                var productoJson = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var nuevoProducto = JsonSerializer.Deserialize<Producto>(productoJson);

                // Validar el modelo antes de agregarlo al inventario
                var validationErrors = ValidarProducto(nuevoProducto);

                if (validationErrors.Count == 0)
                {
                    nuevoProducto.Id = Guid.NewGuid();
                    inventario.Add(nuevoProducto);

                    app.Logger.LogInformation($"Producto añadido - ID: {nuevoProducto.Id}, Nombre: {nuevoProducto.Nombre}, Cantidad: {nuevoProducto.Cantidad}");

                    return Results.Created($"/ObtenerProducto/{nuevoProducto.Id}", nuevoProducto);
                }
                else
                {
                    // Manejar errores de validación
                    return Results.BadRequest(validationErrors);
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError($"Error al agregar el producto: {ex.Message}");
                return Results.Problem("Error interno al agregar el producto", statusCode: 500);
            }
        });
    }

    // Método para validar el producto
    private static List<string> ValidarProducto(Producto producto)
    {
        var errors = new List<string>();

        // Lógica de validación personalizada
        if (string.IsNullOrEmpty(producto.Nombre))
        {
            errors.Add("El nombre del producto es obligatorio.");
        }

        if (producto.Cantidad < 0)
        {
            errors.Add("La cantidad debe ser mayor o igual a cero.");
        }

        // Agregar más reglas de validación según sea necesario

        return errors;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var constructor = WebApplication.CreateBuilder(args);
        var app = constructor.Build();

        app.MapGet("/Ping", () => "Pong");

        RutasInventario.ConfigurarInventario(app);

        app.Run();
    }
}

