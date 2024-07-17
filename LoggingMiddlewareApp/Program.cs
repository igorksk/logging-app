using LoggingMiddlewareApp.Data;
using LoggingMiddlewareApp.Middlewares;
using LoggingMiddlewareApp.Models;
using Microsoft.OpenApi.Models;

namespace LoggingMiddlewareApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<InMemoryDataStore>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IncludeFields = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My CRUD API",
                    Version = "v1",
                    Description = "A minimal CRUD API with Swagger documentation"
                });
            });

            var app = builder.Build();

            app.UseMiddleware<LoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My CRUD API v1");
                c.RoutePrefix = string.Empty;
            });

            // Define your minimal API endpoints here

            app.MapGet("/products", () => InMemoryDataStore.GetProducts());

            app.MapGet("/products/{id}", (int id) =>
            {
                var product = InMemoryDataStore.GetProduct(id);
                return product != null ? Results.Ok(product) : Results.NotFound();
            });

            app.MapPost("/products", (Product product) =>
            {
                InMemoryDataStore.AddProduct(product);
                return Results.Created($"/products/{product.Id}", product);
            });

            app.MapPut("/products/{id}", (int id, Product product) =>
            {
                if (id != product.Id)
                {
                    return Results.BadRequest("Id mismatch in request body and URL");
                }

                InMemoryDataStore.UpdateProduct(product);
                return Results.Ok(product);
            });

            app.MapDelete("/products/{id}", (int id) =>
            {
                InMemoryDataStore.DeleteProduct(id);
                return Results.Ok();
            });

            app.Run();
        }
    }
}
