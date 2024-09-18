namespace OnlineStore.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Admin;
using System.Collections.Generic;

public class AdminService : IAdminService
{
    private readonly IRepository repository;

    public AdminService(IRepository _repository)
    {
        repository = _repository;
    }

    public async Task<bool> BrandExistByIdAsync(int brandId)
    {
       return await repository
            .AllReadOnly<Brand>()
            .AnyAsync(b => b.Id == brandId);
    }

    public async Task<bool> BrandNameExistAsync(string brandName)
    {
        return await repository
            .AllReadOnly<Brand>()
            .AnyAsync(b => b.Name ==  brandName);
    }

    public async Task<bool> ColorExistByIdAsync(int colorId)
    {
        return await repository
            .AllReadOnly<Color>()
            .AnyAsync(c => c.Id == colorId);
    }

    public async Task CreateBrandAsync(BrandFormModel model, string adminId)
    {
        Brand brand = new Brand
        {
          Id = model.Id,
          Name = model.Name,
          ImageUrl = model.ImageUrl,
        };

        await repository.AddAsync(brand);
        await repository.SaveChangesAsync();
    }

    public async Task<bool> ColorNameExistAsync(string colorName)
    {
        return await repository
            .AllReadOnly<Color>()
            .AnyAsync(c => c.Name ==  colorName);
    }

    public async Task CreateColorAsync(ColorFormModel model, string adminId)
    {
        Color color = new Color
        {
            Id = model.Id,
            Name = model.ColorName
        };

        await repository.AddAsync(color);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteColorAsync(int colorId)
    {
        var colorToDelete = await repository
             .All<Color>()
             .Include(p => p.Products)
             .Where(c => c.Id == colorId)
             .ToListAsync();

        if(colorToDelete.Any())
        {
            var productColorsToDelete = await repository
                .All<ProductColor>()
                .Where(pc => pc.ColorId == colorId)
                .ToListAsync();

            if (productColorsToDelete.Any())
            {
                await repository.DeleteRange(productColorsToDelete);
            }

            await repository.DeleteRange(colorToDelete);
            await repository.SaveChangesAsync();
        }
    }


    public async Task<List<ColorPreDeleteViewModel>> GetColorPreDeleteByIdAsync(int colorId)
    {
        return await repository
         .AllReadOnly<Color>()
         .Include(p => p.Products)
         .Select(c => new ColorPreDeleteViewModel
         {
             Id = c.Id,
             Name = c.Name,
             ProductNames = c.Products.Select(p => p.Name).ToList(),
         }).ToListAsync();
    }

    public async Task<List<BrandPreDeleteViewModel>> GetBrandPreDeleteByIdAsync(int brandId)
    {
        return await repository
            .AllReadOnly<Brand>()
            .Include(p => p.Products)
            .Select(b => new BrandPreDeleteViewModel
            {
               Id = b.Id,
               Name = b.Name,
               ImageUrl = b.ImageUrl,
               ProductNames = b.Products.Select(p => p.Name).ToList(),
            }).ToListAsync();
    }

    public async Task DeleteBrandAsync(int brandId)
    {
        var brandToDelete = await repository
         .All<Brand>()
         .Include(b => b.Products)
         .Where(b => b.Id == brandId)
         .ToListAsync();

        if(brandToDelete.Any())
        {
            var productToDeleteBrand = await repository
             .All<Brand>()
             .Include(b => b.Products)
             .FirstOrDefaultAsync(b => b.Id == brandId);

            foreach (var brand in productToDeleteBrand.Products)
            {
                var defaulId = 6;
                brand.BrandId = defaulId;
                await repository.UpdateAsync(brand);
                await repository.SaveChangesAsync();
            }

            await repository.DeleteRange(brandToDelete);
            await repository.SaveChangesAsync();

        }
    }

    public async Task CreateSizeAsync(SizeFormModel model, string adminId)
    {
        Size size = new Size
        {
          Id = model.Id,
          Name = model.Name,
        };

        await repository.AddAsync(size);
        await repository.SaveChangesAsync();
    }

    public async Task<bool> SizeNameExistAsync(string sizeName)
    {
        return await repository
            .AllReadOnly<Size>()
            .AnyAsync(s => s.Name ==  sizeName);
    }

    public async Task<bool> SizeExistByIdAsync(int sizeId)
    {
        return await repository
            .AllReadOnly<Size>()
            .AnyAsync(s => s.Id == sizeId);
    }

    public async Task<List<SizePreDeleteViewModel>> GetSizePreDeleteByIdAsync(int sizeId)
    {
        return await repository
            .AllReadOnly<Size>()
            .Include(p => p.Products)
            .Select(s => new SizePreDeleteViewModel
            {
                Id = s.Id,
                Name = s.Name,
                ProductNames = s.Products.Select(p => p.Name).ToList(),
            }).ToListAsync();
    }

    public async Task DeleteSizeAsync(int sizeId)
    {
        var sizesToDelete = await repository
             .All<Size>()
             .Include(p => p.Products)
             .Where(s => s.Id == sizeId)
             .ToListAsync();

        if(sizesToDelete.Any())
        {
            var productSizesToDelete = await repository
                .All<ProductSize>()
                .Where(ps => ps.SizeId == sizeId)
                .ToListAsync();

            if(productSizesToDelete.Any())
            {
                await repository.DeleteRange(productSizesToDelete);
            }

            await repository.DeleteRange(sizesToDelete);
            await repository.SaveChangesAsync();
        }
    }

    public async Task CreateCategoryAsync(CategoryFormModel model, string adminId)
    {
        Category category = new Category
        {
          Id = model.Id,
          Name = model.Name,
        };

        await repository.AddAsync(category);
        await repository.SaveChangesAsync();
    }

    public async Task<bool> CategoryNameExistAsync(string categoryName)
    {
        return await repository
            .AllReadOnly<Category>()
            .AnyAsync(c => c.Name == categoryName);
    }

    public async Task<bool> CategoryExistByIdAsync(int categoryId)
    {
        return await repository
            .AllReadOnly<Category>()
            .AnyAsync(c => c.Id == categoryId);
    }

    public async Task<List<CategoryPreDeleteViewModel>> GetCategoryPreDeleteByIdAsync(int categoryId)
    {
        return await repository
            .AllReadOnly<Category>()
            .Include(p => p.Products)
            .Select(c => new CategoryPreDeleteViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ProductNames = c.Products.Select(p => p.Name).ToList()
            }).ToListAsync();
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var categoriesToDelete = await repository
            .All<Category>()
            .Include(p => p.Products)
            .Where(c => c.Id == categoryId)
            .ToListAsync();

        if(categoriesToDelete.Any())
        {
            var defaultCategoryId = 4;

            var productsToDeleteCategpry = await repository
                    .All<Category>()
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(c => c.Id == categoryId);


            foreach(var category in productsToDeleteCategpry.Products)
            {
                category.CategoryId = defaultCategoryId;
                await repository.UpdateAsync(category);
                await repository.SaveChangesAsync();
            }

            await repository.DeleteRange(categoriesToDelete);
            await repository.SaveChangesAsync();
        }
    }
}
