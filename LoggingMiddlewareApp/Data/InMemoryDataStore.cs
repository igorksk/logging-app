using LoggingMiddlewareApp.Models;

namespace LoggingMiddlewareApp.Data
{
    public class InMemoryDataStore
    {
        public static List<Product> Products { get; } = new List<Product>()
    {
        new Product { Id = 1, Name = "Product 1", Price = 10.00m },
        new Product { Id = 2, Name = "Product 2", Price = 20.00m }
    };

        public static Product? GetProduct(int id)
        {
            return Products.FirstOrDefault(p => p.Id == id);
        }

        public static List<Product> GetProducts()
        {
            return Products.ToList();
        }

        public static void AddProduct(Product product)
        {
            product.Id = Products.Max(p => p.Id) + 1;
            Products.Add(product);
        }

        public static void UpdateProduct(Product product)
        {
            var existingProduct = Products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
            }
        }

        public static void DeleteProduct(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                Products.Remove(product);
            }
        }
    }
}
