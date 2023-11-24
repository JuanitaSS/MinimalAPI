using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalAPI.Modelos;
using Microsoft.AspNetCore.Mvc;

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


                
                var validationResults = new List<ValidationResult>();
                if (Validator.TryValidateObject(nuevoProducto, new ValidationContext(nuevoProducto), validationResults, true))
                {
                    nuevoProducto.Id = Guid.NewGuid();
                    inventario.Add(nuevoProducto);

                    app.Logger.LogInformation($"Producto añadido - ID: {nuevoProducto.Id}, Nombre: {nuevoProducto.Nombre}, Cantidad: {nuevoProducto.Cantidad}");

                    return Results.Created($"/ObtenerProducto/{nuevoProducto.Id}", nuevoProducto);
                }
                else
                {
                    
                    return Results.BadRequest(validationResults);
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError($"Error al agregar el producto: {ex.Message}");
                return Results.Problem("Error interno al agregar el producto", statusCode: 500);
            }
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
