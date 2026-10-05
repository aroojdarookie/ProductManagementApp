using System.Text.Json;
using ProductManagementApp.DataAccessLayer.Interfaces;
using ProductManagementApp.Models;

namespace ProductManagementApp.DataAccessLayer.Repositories;

public class ProductRepositoryJson : IProductRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public ProductRepositoryJson()
    {
        _filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "products.json"
        );
    }

    private async Task<List<ProductModel>> ReadProductsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<ProductModel>();
        }

        var json = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<ProductModel>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<ProductModel>>(
                json,
                _jsonOptions
            ) ?? new List<ProductModel>();
        }
        catch (JsonException)
        {
            throw new InvalidDataException(
                "The products.json file contains invalid JSON."
            );
        }
    }

    private async Task WriteProductsAsync(List<ProductModel> products)
    {
        var json = JsonSerializer.Serialize(
            products,
            _jsonOptions
        );

        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<ProductModel>> GetAllAsync()
    {
        return await ReadProductsAsync();
    }

    public async Task<ProductModel?> GetByIdAsync(Guid id)
    {
        var products = await ReadProductsAsync();

        return products.FirstOrDefault(
            p => p.Id == id
        );
    }

    public async Task AddAsync(ProductModel product)
    {
        var products = await ReadProductsAsync();

        products.Add(product);

        await WriteProductsAsync(products);
    }

    public async Task UpdateAsync(ProductModel product)
    {
        var products = await ReadProductsAsync();

        var existing = products.FirstOrDefault(
            p => p.Id == product.Id
        );

        if (existing == null)
        {
            throw new KeyNotFoundException(
                "Product not found."
            );
        }

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Category = product.Category;
        existing.Description = product.Description;
        existing.IsActive = product.IsActive;

        await WriteProductsAsync(products);
    }

    public async Task DeleteAsync(Guid id)
    {
        var products = await ReadProductsAsync();

        var product = products.FirstOrDefault(
            p => p.Id == id
        );

        if (product == null)
        {
            return;
        }

        products.Remove(product);

        await WriteProductsAsync(products);
    }

    public async Task<List<ProductModel>> SearchAsync(
        string keyword)
    {
        var products = await ReadProductsAsync();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return products;
        }

        keyword = keyword.Trim();

        return products
            .Where(p =>
                p.Name.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                p.Category.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                p.Description.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            .ToList();
    }
}
