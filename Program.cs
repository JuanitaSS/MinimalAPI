using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Modelos;
using MinimalAPI.Validaciones;

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

            // Validar el modelo antes de agregarlo al inventario
            var validationResults = Validaciones.ValidarProducto(nuevoProducto);

            if (validationResults.Any())
            {
                // Manejar errores de validación
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonSerializer.Serialize(validationResults));
                return;
            }

            // Continuar con la lógica de agregar el producto
            nuevoProducto.Id = Guid.NewGuid().ToString();
            inventario.Add(nuevoProducto);

            app.Logger.LogInformation($"Producto añadido - ID: {nuevoProducto.Id}, Nombre: {nuevoProducto.Nombre}, Cantidad: {nuevoProducto.Cantidad}");

            context.Response.StatusCode = 201;
            await context.Response.WriteAsync(JsonSerializer.Serialize(nuevoProducto));
        });
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Bienvenido al inventario");

// Configuración de otras rutas
RutasInventario.ConfigurarInventario(app);

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
