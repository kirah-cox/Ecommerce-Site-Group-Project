
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;

public class Product
{
    private static string GetProductsJsonPath()
    {
        // Get the base directory where the application is running
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        // Navigate to the project root (typically up from bin/Debug/net10.0)
        var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
        return Path.Combine(projectRoot, "products.json");
    }

    public int Version { get; set; }
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }

    public Product()
    {
        Version = 1;
        Id = Guid.NewGuid();
    }

    private static async Task<List<Product>> LoadProductsAsync()
    {
        var filePath = GetProductsJsonPath();
        if (!File.Exists(filePath)) 
            return new List<Product>();
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
    }

    private static async Task SaveProductsAsync(List<Product> products)
    {
        var filePath = GetProductsJsonPath();
        var json = JsonSerializer.Serialize(products);
        await File.WriteAllTextAsync(filePath, json);
    }

    // Instance helpers to persist this product (similar pattern to User)
    public async Task Add()
    {
        var items = await LoadProductsAsync();
        items.Add(this);
        await SaveProductsAsync(items);
    }

    public async Task<bool> Update()
    {
        var items = await LoadProductsAsync();
        int idx = items.FindIndex(p => p.Id == Id);
        if (idx == -1) return false;
        items[idx] = this;
        await SaveProductsAsync(items);
        return true;
    }

    public async Task<bool> Delete()
    {
        var items = await LoadProductsAsync();
        int idx = items.FindIndex(p => p.Id == Id);
        if (idx == -1) return false;
        items.RemoveAt(idx);
        await SaveProductsAsync(items);
        return true;
    }

    // Product collection static helpers
    public static class ProductCollection
    {
        public static Task<List<Product>> LoadAsync() => LoadProductsAsync();
        public static Task SaveAsync(List<Product> products) => SaveProductsAsync(products);

        public static async Task AddAsync(Product product)
        {
            var items = await LoadProductsAsync();
            items.Add(product);
            await SaveProductsAsync(items);
        }

        public static async Task<bool> UpdateAsync(Product product)
        {
            var items = await LoadProductsAsync();
            int idx = items.FindIndex(p => p.Id == product.Id);
            if (idx == -1) return false;
            items[idx] = product;
            await SaveProductsAsync(items);
            return true;
        }

        public static async Task<bool> DeleteAsync(Guid id)
        {
            var items = await LoadProductsAsync();
            int idx = items.FindIndex(p => p.Id == id);
            if (idx == -1) return false;
            items.RemoveAt(idx);
            await SaveProductsAsync(items);
            return true;
        }
    }
}
