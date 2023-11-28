using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MinimalAPI.Modelos;

namespace MinimalAPI.Routes
{
    public static class OtrasRutas
    {
        public static void Otra(WebApplication app)
        {
            app.MapGet("/Ping", () => "Pong");

            app.MapPost("/CustomJSON", (dynamic json) =>
            {
                Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                string value1 = dict["value1"];

                return Results.Ok($"Value1: {value1}");
            });

            List<Producto> inventario = new List<Producto>();

            app.MapGet("/ObtenerInventario", () => Results.Json(inventario));

            app.MapPost("/AgregarProducto", async (HttpContext context) =>
            {
                var productoJson = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var nuevoProducto = JsonSerializer.Deserialize<Producto>(productoJson);

                nuevoProducto.Id = Guid.NewGuid();
                inventario.Add(nuevoProducto);

                app.Logger.LogInformation($"Producto añadido - ID: {nuevoProducto.Id}, Nombre: {nuevoProducto.Nombre}, Cantidad: {nuevoProducto.Cantidad}");

                return Results.Created($"/ObtenerProducto/{nuevoProducto.Id}", nuevoProducto);
            });
        }
    }
}
