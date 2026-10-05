using ProductManagementApp.Business.Interfaces;
using ProductManagementApp.DataAccessLayer.Interfaces;
using ProductManagementApp.Models;

namespace ProductManagementApp.Business.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductModel>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProductModel?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(ProductModel product)
    {
        ValidateProduct(product);

        product.Id = Guid.NewGuid();

        await _repository.AddAsync(product);
    }

    public async Task UpdateAsync(ProductModel product)
    {
        ValidateProduct(product);

        await _repository.UpdateAsync(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<List<ProductModel>> SearchAsync(
        string keyword)
    {
        return await _repository.SearchAsync(keyword);
    }

    private static void ValidateProduct(
        ProductModel product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException(
                "Name is required."
            );
        }

        if (product.Name.Trim().Length < 3)
        {
            throw new ArgumentException(
                "Name must contain at least 3 characters."
            );
        }

        if (product.Price <= 0)
        {
            throw new ArgumentException(
                "Price must be greater than zero."
            );
        }

        if (string.IsNullOrWhiteSpace(product.Category))
        {
            throw new ArgumentException(
                "Category is required."
            );
        }

        if (product.Description.Length > 500)
        {
            throw new ArgumentException(
                "Description cannot exceed 500 characters."
            );
        }
    }
}