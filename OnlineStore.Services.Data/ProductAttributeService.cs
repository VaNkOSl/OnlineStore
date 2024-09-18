namespace OnlineStore.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Services.Data.Contacts;
public class ProductAttributeService : IProductAttributeService
{
    private readonly IRepository repository;

    public ProductAttributeService(IRepository _repository)
    {
        repository = _repository; 
    }
    public async Task<IEnumerable<string>> AllBrandsNameAsync()
    {
        return await repository
            .AllReadOnly<Brand>()
            .Select(b => b.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> AllCategoriesNamesAsync()
    {
        return await repository
            .AllReadOnly<Category>()
            .Select(c => c.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> AllColorsNameAsync()
    {
        return await repository
            .AllReadOnly<Color>()
            .Select(c => c.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> AllSizesNameAsync()
    {
        return await repository
            .AllReadOnly<Size>()
            .Select(s => s.Name)
            .Distinct()
            .ToListAsync();
    }
}
