using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Models;

namespace MinimalAPI.Routes
{
    public static class AnotherRoutes
    {
        public static void Another(WebApplication app)
        {
            List<Product> inventory = new List<Product>();

            app.MapGet("/Ping", () => "Pong");

            app.MapPost("/CustomJSON", (dynamic json) =>
            {
                Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                string value1 = dict["value1"];

                return Results.Ok($"Value1: {value1}");
            });

            app.MapGet("/GetInventory", () => Results.Json(inventory));

            app.MapPost("/AddProduct", async (HttpContext context) =>
            {
                var productJson = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var newProduct = JsonSerializer.Deserialize<Product>(productJson);

                var validationResults = Validations.ValidarProduct(newProduct);

                if (validationResults.Count > 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(validationResults));
                    return;
                }

                newProduct.Id = Guid.NewGuid().ToString();
                inventory.Add(newProduct);

                app.Logger.LogInformation($"Product added - ID: {newProduct.Id}, Name: {newProduct.Name}, Quantity: {newProduct.Quantity}");

                context.Response.StatusCode = 201;
                await context.Response.WriteAsync(JsonSerializer.Serialize(newProduct));
            });
        }
    }
}

