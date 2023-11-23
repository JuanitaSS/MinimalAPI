/*
WebApplicationBuilder builder = WebApplication.CreateBuilder(args)
WebApplication app = builder.Build()
*/

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class Producto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public int Cantidad { get; set; }
    // Puedes agregar más propiedades según tus necesidades
}

public static class RutasInventario
{
    public static void ConfigurarInventario(WebApplication app)
    {
        List<Producto> inventario = new List<Producto>();

        app.MapGet("/", () => "Bienvenido al inventario");

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
