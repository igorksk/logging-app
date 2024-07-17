using System.Text.Json.Serialization;

namespace LoggingMiddlewareApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
}
