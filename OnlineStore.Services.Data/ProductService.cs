namespace OnlineStore.Services.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Admin;
using OnlineStore.Web.ViewModels.Products;
using OnlineStore.Web.ViewModels.Products.Enums;
using OnlineStore.Web.ViewModels.Reviews;
using OnlineStore.Web.ViewModels.Sellers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProductService : IProductService
{
    private readonly IRepository repository;

    public ProductService(IRepository _repository)
    {
        repository = _repository;
    }
    public async Task<IEnumerable<ProductBrandServiceModel>> AllBrandsAsync()
    {
        return await repository
            .AllReadOnly<Brand>()
            .Select(b => new ProductBrandServiceModel
            {
                Id = b.Id,
                Name = b.Name,
                ImageUrl = b.ImageUrl
            }).ToListAsync();
    }

    public async Task<IEnumerable<ProductCategoryServiceModel>> AllCategoriesAsync()
    {
        return await repository
             .AllReadOnly<Category>()
             .Select(c => new ProductCategoryServiceModel
             {
                 Id = c.Id,
                 Name = c.Name,
             }).ToListAsync(); ;
    }

    public async Task<IEnumerable<ProductColorServiceModel>> AllColorsAsync()
    {
        return await repository
            .AllReadOnly<Color>()
            .Select(pc => new ProductColorServiceModel
            {
                ColorId = pc.Id,
                ColorName = pc.Name,
            }).ToListAsync();
    }

    public async Task<IEnumerable<ProductSizeServiceModel>> AllSizesAsync()
    {
        return await repository
            .AllReadOnly<Size>()
            .Select(ps => new ProductSizeServiceModel
            {
                SizeId = ps.Id,
                SizeName = ps.Name
            }).ToListAsync();
    }

    public async Task<bool> BrandExistsAsync(int brandId)
    {
        return await repository
            .AllReadOnly<Brand>()
            .AnyAsync(b => b.Id == brandId);
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await repository
            .AllReadOnly<Category>()
            .AnyAsync(c => c.Id == categoryId);
    }

    public async Task<bool> ColorExistsAsync(IEnumerable<int> colorId)
    {
        foreach (var colorid in colorId)
        {
            if (!await repository.AllReadOnly<Color>().AnyAsync(c => c.Id == colorid))
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> SizeExistsAsync(IEnumerable<int> sizeIds)
    {
        foreach (var sizeId in sizeIds)
        {
            if (!await repository.AllReadOnly<Size>().AnyAsync(s => s.Id == sizeId))
            {
                return false;
            }
        }
        return true;
    }

    public async Task<string> CreateProductAsync(ProductFormModel model, string sellerId, List<int> selectedColors, List<int> selectedSizes, IFormFileCollection images)
    {
        Product product = new Product
        {
            Name = model.Name,
            BrandId = model.BrandId,
            CategoryId = model.CategoryId,
            DateAdded = DateTime.Now,
            StockQuantity = model.StockQuantity,
            Description = model.Description,
            IsApproved = false,
            IsAvaible = true,
            SuitableSeason = model.SuitableSeason,
            Price = model.Price,
            SellerId = Guid.Parse(sellerId),
        };

        foreach (var colorId in selectedColors)
        {
            var color = await repository.GetByIdAsync<Color>(colorId);
            if (color != null)
            {
                product.AvailableColors.Add(new ProductColor { ColorId = color.Id, ProductId = product.Id});

                if(!color.Products.Contains(product))
                {
                    color.Products.Add(product);
                }
            }

            else
            {
                throw new ArgumentException("Invalid color Id, please try again later");
            }
        }

        foreach (var sizeId in selectedSizes)
        {
            var size = await repository.GetByIdAsync<Size>(sizeId);
            if (size != null)
            {
                product.AvailableSizes.Add(new ProductSize { SizeId = size.Id, ProductId = product.Id });

                if(!size.Products.Contains(product))
                {
                    size.Products.Add(product);
                }
            }
            else
            {
                throw new ArgumentException("Invalid size Id, please try again later");
            }
        }

        foreach (var image in images)
        {
            var uploadDirectory = Path.Combine("wwwroot", "uploads");

            if(!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var fileNameWithoutExtension = Path.GetFileName(image.FileName);
            var extensions = Path.GetExtension(image.FileName);

            var newFileName = $"{fileNameWithoutExtension}_{product.Id}{extensions}";
            var filePath = Path.Combine(uploadDirectory, newFileName);

            using(var stream = new FileStream(filePath,FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            product.ProductImages.Add(new ProductImage
            {
                FilePath = "/uploads/" + newFileName,
               ProductId = product.Id
            });
        }

        await repository.AddAsync(product);
        await repository.SaveChangesAsync();
        return product.Id.ToString();
    }

    public async Task<ProductQueryServiceModel> AllProductsAsync(AllProductsQueryModel model)
    {
        IQueryable<Product> productAsQurable = repository
            .AllReadOnly<Product>()
            .Where(p => p.IsApproved == true)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.Category))
        {
            productAsQurable = productAsQurable
                .Where(c => c.Category.Name == model.Category);
        }

        if (!string.IsNullOrWhiteSpace(model.Brand))
        {
            productAsQurable = productAsQurable
                .Where(b => b.Brand.Name == model.Brand);
        }

        if (!string.IsNullOrWhiteSpace(model.SearchinString))
        {
            string wildCard = $"%{model.SearchinString.ToLower()}%";

            productAsQurable = productAsQurable
                .Where(x => EF.Functions.Like(x.Name, wildCard) ||
                            EF.Functions.Like(x.Description, wildCard));
        }

        if (!string.IsNullOrWhiteSpace(model.Color))
        {
            productAsQurable = productAsQurable
                .Where(c => c.AvailableColors.Any(c => c.Color.Name == model.Color));
        }

        if (!string.IsNullOrWhiteSpace(model.Size))
        {
            productAsQurable = productAsQurable
                .Where(s => s.AvailableSizes.Any(s => s.Size.Name == model.Size));
        }

        productAsQurable = model.ProductSorting switch
        {
            ProductSorting.Newest => productAsQurable
                                            .OrderByDescending(p => p.DateAdded),

            ProductSorting.Oldest => productAsQurable
                                            .OrderBy(p => p.DateAdded),

            ProductSorting.LowPrice => productAsQurable
                                               .OrderBy(p => p.Price),
            ProductSorting.HighPrice => productAsQurable
                                               .OrderByDescending(p => p.Price),
            _ => productAsQurable
        };

        IEnumerable<ProductServiceModel> allProducts = await productAsQurable
           .Skip((model.CurrentPage - 1) * model.ProductPerPage)
           .Take(model.ProductPerPage)
           .Select(p => new ProductServiceModel
           {
               Id = p.Id.ToString(),
               Name = p.Name,
               IsAvaible = p.IsAvaible,
               Images = p.ProductImages.Select(pi => new ProductImageServiceModel
               {
                   Id = pi.Id.ToString(),
                   ProductId = p.Id.ToString(),
               }),
               Price = p.Price,
               FirstImageUrl = p.ProductImages.Select(pi => pi.FilePath).FirstOrDefault() ?? string.Empty
           }).ToListAsync();
        int totalProduct = productAsQurable.Count();

        return new ProductQueryServiceModel
        {
            TotalProductsCount = totalProduct,
            Products = allProducts,
        };

    }

    public async Task<ProductDetailsViewModel> GetProductDetailsByIdAsync(string productId)
    {
        Product product = await repository
       .All<Product>()
       .Include(c => c.Category)
       .Include(b => b.Brand)
       .Include(p => p.AvailableColors)
       .ThenInclude(pc => pc.Color)
       .Include(p => p.AvailableSizes)
       .ThenInclude(ps => ps.Size)
       .Include(pi => pi.ProductImages)
       .Include(s => s.Seller)
       .ThenInclude(u => u.User)
       .Include(r => r.Reviews)
       .ThenInclude(u => u.User)
       .FirstAsync(p => p.Id.ToString() == productId);

        return new ProductDetailsViewModel
        {
            Id = product.Id.ToString(),
            Description = product.Description,
            Name = product.Name,
            StockQuantity = product.StockQuantity,
            Price = product.Price,
            SuitableSeason = product.SuitableSeason,
            IsAvaible = product.IsAvaible,
            CategoryName = product.Category.Name,
            BrandName = product.Brand.Name,
            Seller = new SellerServiceModel
            {
                FullName = $"{product.Seller.FirstName} {product.Seller.LastName}",
                PhoneNumber = product.Seller.PhoneNumber,
                Email = product.Seller.User.Email ?? string.Empty
            },
            Colors = product.AvailableColors.Select(pc => pc.Color.Name).ToList(),
            Sizes = product.AvailableSizes.Select(ps => ps.Size.Name).ToList(),
            ImagePaths = product.ProductImages.Select(pi => pi.FilePath).ToList(),
            Reviews = product.Reviews.Select(r => new ProductReviewViewModel
            {
                Content = r.Content,
                Rating = r.Rating,
                ReviewDate = r.ReviewDate,
                UserFullName = r.User.FirstName + " " + r.User.LastName,
            }).ToList()
        };
    }

    public async Task<bool> ProductExistsAsync(string productId)
    {
        return await repository
            .AllReadOnly<Product>()
            .AnyAsync(p => p.Id.ToString() == productId);
    }

    public async Task<string> CreateProductReviewAsync(ProductReviewViewModel model, string userId)
    {
        if (!Guid.TryParse(model.ProductId, out Guid prodcutIdGuid))
        {
            throw new ArgumentException("Invalid Product Id");
        }

        var product = await repository
            .AllReadOnly<Product>()
            .FirstOrDefaultAsync(p => p.Id == prodcutIdGuid);

        var user = await repository
            .AllReadOnly<ApplicationUser>()
            .FirstOrDefaultAsync(ap => ap.Id.ToString() == userId);

        if(user == null)
        {
            throw new ArgumentException("User not found");
        }

        if (product == null)
        {
            throw new ArgumentException("Product not found!");
        }

        Review review = new Review
        {
            Rating = model.Rating,
            Content = model.Content,
            ReviewDate = model.ReviewDate == default ? DateTime.UtcNow : model.ReviewDate,
            UserId = Guid.Parse(userId),
            ProductId = prodcutIdGuid
        };

        if(!user.Reviews.Contains(review))
        {
            user.Reviews.Add(review);
        }

        if(!product.Reviews.Contains(review))
        {
            product.Reviews.Add(review);
        }

        await repository.AddAsync(review);
        await repository.SaveChangesAsync();

        return review.Id.ToString();
    }

    public async Task<IEnumerable<ProductServiceModel>> AllProductsBySellerIdAsync(string sellerId)
    {
        IEnumerable<ProductServiceModel> allProducts = await repository
            .All<Product>()
            .Where(p => p.SellerId.ToString() == sellerId)
           .Select(p => new ProductServiceModel
           {
               Id = p.Id.ToString(),
               Name = p.Name,
               IsAvaible = p.IsAvaible,
               Images = p.ProductImages.Select(pi => new ProductImageServiceModel
               {
                   Id = pi.Id.ToString(),
                   ProductId = p.Id.ToString(),
               }),
               IsApproved = p.IsApproved,
               RejectionReason = p.RejectionReason ?? "This product has no reason left for its refusal",
               Price = p.Price,
               FirstImageUrl = p.ProductImages.Select(pi => pi.FilePath).FirstOrDefault() ?? string.Empty
           }).ToListAsync();

        return allProducts;
    }

    public async Task<ProductPreDeleteViewModel> GetProductForDeletingByIdAsync(string productId)
    {
        var product = await repository
         .AllReadOnly<Product>()
         .Include(p => p.ProductImages)
         .FirstOrDefaultAsync(p => p.Id.ToString() == productId);

        if (product == null)
        {
            throw new ArgumentException("Product not found!");
        }

        return new ProductPreDeleteViewModel
        {
            Name = product.Name,
            Price = product.Price,
            Image = product.ProductImages.FirstOrDefault()?.FilePath ?? string.Empty,
            ProductImages = product.ProductImages.Select(pi => new ProductImageServiceModel
            {
                FilePath = pi.FilePath,
                ProductId = product.Id.ToString(),
            }).ToList()
        };
    }

    public async Task DeleteProductByIdAsync(string productId)
    {
        if (!Guid.TryParse(productId, out Guid productGuid))
        {
            throw new ArgumentException("Invalid product ID format.");
        }

        Product productToDelete = await repository
            .All<Product>()
            .Include(r => r.Reviews)
            .Include(pc => pc.AvailableColors)
            .Include(ps => ps.AvailableSizes)
            .Include(pi => pi.ProductImages)
            .FirstAsync(p => p.Id == productGuid);

        if (productToDelete != null)
        {
            bool productHasReview = await ProductHasReviewAsync(productToDelete.Id.ToString());

            if (productHasReview == true)
            {
                await DeleteProductReviewAsync(productToDelete.Id.ToString());
            }

            await DeleteProductCartItemsByIdAsync(productToDelete.Id.ToString());
            await DeteleProductSizesByIdAsync(productToDelete.Id.ToString());
            await DeleteProductColorsByIdAsync(productToDelete.Id.ToString());
            await DeleteProductImagesAsync(productToDelete.Id.ToString());

            await repository.DeleteAsync<Product>(productGuid);
            await repository.SaveChangesAsync();
        }
    }

    public async Task DeleteProductReviewAsync(string productId)
    {
        var reviewToDelete = await repository
            .All<Review>()
            .Where(p => p.ProductId.ToString() == productId)
            .ToListAsync();

        if (reviewToDelete.Any())
        {
            await repository.DeleteRange(reviewToDelete);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<bool> ProductHasReviewAsync(string productId)
    {
        return await repository
            .AllReadOnly<Review>()
            .AnyAsync(p => p.ProductId.ToString() == productId);
    }

    public async Task DeteleProductSizesByIdAsync(string productId)
    {
        var sizesToDelete = await repository
            .All<ProductSize>()
            .Where(p => p.ProductId.ToString() == productId)
            .ToListAsync();

        if (sizesToDelete.Any())
        {
            await repository.DeleteRange(sizesToDelete);
            await repository.SaveChangesAsync();
        }
    }

    public async Task DeleteProductColorsByIdAsync(string productId)
    {
        var colorsToDelete = await repository
            .All<ProductColor>()
            .Where(p => p.ProductId.ToString() == productId)
            .ToListAsync();

        if (colorsToDelete.Any())
        {
            await repository.DeleteRange(colorsToDelete);
            await repository.SaveChangesAsync();
        }
    }

    public async Task DeleteProductImagesAsync(string productId)
    {
        var imagesToDelete = await repository
          .All<ProductImage>()
          .Where(p => p.ProductId.ToString() == productId)
          .ToListAsync();

        if (imagesToDelete.Any())
        {
            foreach (var image in imagesToDelete)
            {
                var fileName = Path.GetFileName(image.FilePath);
                var uploadDirectory = Path.Combine("wwwroot", "uploads");
                var filePath = Path.Combine(uploadDirectory, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            await repository.DeleteRange(imagesToDelete);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<ProductFormModel> GetProductForEditByIdAsync(string productId)
    {
        var productToEdit = await repository
            .All<Product>()
            .Include(c => c.Category)
            .Include(b => b.Brand)
            .Include(c => c.AvailableColors)
            .Include(s => s.AvailableSizes)
            .Include(pi => pi.ProductImages)
            .FirstAsync(p => p.Id.ToString() == productId);

        if (productToEdit != null)
        {
            var categories = await AllCategoriesAsync();
            var brands = await AllBrandsAsync();
            var colors = await AllColorsAsync();
            var sizes = await AllSizesAsync();

            return new ProductFormModel
            {
                Name = productToEdit.Name,
                Description = productToEdit.Description,
                Brands = brands,
                Colors = colors,
                Price = productToEdit.Price,
                Sizes = sizes,
                Categories = categories,
                StockQuantity = productToEdit.StockQuantity,
                SuitableSeason = productToEdit.SuitableSeason,
            };
        }

        return null!;
    }

    public async Task EditProductByFormModelAsync(ProductFormModel model, string productId, List<int> selectedColorIds, List<int> selectedSizeIds, IFormFileCollection images)
    {
        var product = await repository
            .All<Product>()
            .Include(c => c.AvailableColors)
            .Include(s => s.AvailableSizes)
            .Include(pi => pi.ProductImages)
            .Include(s => s.Seller)
            .FirstOrDefaultAsync(p => p.Id.ToString() == productId);

        if (product == null)
        {
            throw new ArgumentException($"Product with id {productId} not found!");
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.StockQuantity = model.StockQuantity;
        product.BrandId = model.BrandId;
        product.CategoryId = model.CategoryId;
        product.SuitableSeason = model.SuitableSeason;
        product.IsApproved = false;

        var existingColorIds = product.AvailableColors.Select(pc => pc.ColorId).ToList();
        var colorsToAdd = selectedColorIds.Except(existingColorIds).ToList();
        var colorsToRemove = existingColorIds.Except(selectedColorIds).ToList();

        foreach (var colorId in colorsToRemove)
        {
            var colorToRemove = product
                .AvailableColors
                .FirstOrDefault(pc => pc.ColorId == colorId);

            if (colorToRemove != null)
            {
                product.AvailableColors.Remove(colorToRemove);
            }
        }

        foreach(var colorId in colorsToAdd)
        {
            var colorToAdd = await repository
                .All<Color>()
                .FirstOrDefaultAsync(c => c.Id == colorId);

            if(colorToAdd != null)
            {
                var productColor = new ProductColor { ColorId = colorId, ProductId = product.Id };
                await repository.AddAsync(productColor);
            }
        }

        var existingSizeId = product.AvailableSizes.Select(ps => ps.SizeId).ToList();
        var sizesToAdd = selectedSizeIds.Except(existingSizeId).ToList();
        var sizesToRemove = existingSizeId.Except(selectedSizeIds).ToList();

        foreach (var sizeId in sizesToRemove)
        {
            var sizeToremove = product
                .AvailableSizes
                .FirstOrDefault(ps => ps.SizeId == sizeId);

            if(sizesToRemove != null)
            {
                product.AvailableSizes.Remove(sizeToremove!);
            }
        }

        foreach(var sizeId in sizesToAdd)
        {
            var sizeToAdd = await repository
                .All<Size>()
                .FirstOrDefaultAsync(s => s.Id == sizeId);

            if(sizesToAdd != null)
            {
                var productSize = new ProductSize { SizeId = sizeId, ProductId = product.Id };
                await repository.AddAsync(productSize);
            }
        }

        if(images != null && images.Any())
        {
            var existingImages = product
                .ProductImages
                .Select(pi => pi.FilePath).ToList();

            var imagesToAdd = images;

            var imagesToRemove = product
                .ProductImages
                .Where(pi => !images.Any(i => i.FileName == pi.FilePath.Substring(9))).ToList();

            foreach (var imageToRemove in imagesToRemove)
            {
                product.ProductImages.Remove(imageToRemove);
                var filePath = Path.Combine("wwwroot/uploads", imageToRemove.FilePath.Substring(9));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            foreach (var image in imagesToAdd)
            {
                if (image.Length > 0)
                {
                    var uploadDirectory = Path.Combine("wwwroot", "uploads");

                    var fileNameWithoutExtension = Path.GetFileName(image.FileName);
                    var extensions = Path.GetExtension(image.FileName);

                    var newFileName = $"{fileNameWithoutExtension}_{product.Id}{extensions}";
                    var filePath = Path.Combine(uploadDirectory, newFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var producImages = new ProductImage
                    {
                        FilePath = "/uploads/" + newFileName,
                        ProductId = product.Id
                    };

                    await repository.AddAsync(producImages);
                }
            }
        }

        await repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductIndexServiceModel>> LastProductsAsync()
    {
        var products =  await repository
            .AllReadOnly<Product>()
            .Where(p => p.IsAvaible == true && p.IsApproved == true)
            .Select(p => new ProductIndexServiceModel
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                ProductImage = p.ProductImages.Select(pi => pi.FilePath).FirstOrDefault() ?? string.Empty,
            }).ToListAsync();

        return products;
    }

    public async Task ApplyDiscountAsync(string productId, int discountPercentage)
    {
        var product = await GetProductByIdAsync(productId);

        if (product == null)
        {
            throw new ArgumentException($"Product with id {productId} not found!");
        }

        product.Price -= product.Price * discountPercentage / 100.0m;
        await repository.UpdateAsync(product);
        await repository.SaveChangesAsync();
    }

    public async Task<Product> GetProductByIdAsync(string productId)
    {
        var product =  await repository
            .AllReadOnly<Product>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == productId);

        if(product == null)
        {
            return null!;
        }

        return product;
    }

    public async Task<bool> ProductHasOrdersAsync(string productId)
    {
        var orders = await repository
            .AllReadOnly<Order>()
            .Include(or => or.OrderItems)
            .ThenInclude(p => p.Product)
            .Where(o => o.OrderStatus != OrderStatus.ReadyForPickup)
            .Select(or => or.OrderItems.Any(p => p.ProductId.ToString() == productId))
            .ToListAsync();

        if(orders.Count > 0)
        {
            return true;
        }

        return false;
    }

    public async Task DeleteProductCartItemsByIdAsync(string productId)
    {
        var cartItemToDelete = await repository
            .All<CartItem>()
            .Where(p => p.ProductId.ToString() == productId)
            .ToListAsync();

        if(cartItemToDelete != null)
        {
            await repository.DeleteRange(cartItemToDelete);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<string> GetProductNameAsync(string productId)
    {
        var product = await repository
            .AllReadOnly<Product>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == productId);

        if(product == null)
        {
            return "Unknown product!";
        }

        return product.Name;
    }

    public async Task<List<ProductForApproveServiceModel>> GetUnapprovedProductsAsync()
    {
        return await repository
             .AllReadOnly<Product>()
             .Where(p => !p.IsApproved && string.IsNullOrWhiteSpace(p.RejectionReason))
             .Select(p => new ProductForApproveServiceModel
             {
               Id = p.Id.ToString(),
               Name = p.Name,
               Price = p.Price,
               StockQuantity = p.StockQuantity,
               FirstImage = p.ProductImages.Select(pi =>pi.FilePath).FirstOrDefault() ?? string.Empty,
               BrandName = p.Brand.Name,
               CategoryName = p.Category.Name,
               Seller = new SellerServiceModel
               {
                  FullName = $"{p.Seller.FirstName} {p.Seller.LastName}",
                  PhoneNumber = p.Seller.PhoneNumber,
                  Email = p.Seller.User.Email ?? string.Empty
               },
               Colors = p.AvailableColors.Select(ac => ac.Color.Name).ToList(),
               Sizes = p.AvailableSizes.Select(s => s.Size.Name).ToList(),
             })
             .ToListAsync();
    }

    public async Task ApproveProductAsync(string productId)
    {
        Guid productGuidId = Guid.Parse(productId);

        var product = await repository
            .GetByIdAsync<Product>(productGuidId);

        if(product != null && product.IsApproved == false)
        {
            product.IsApproved = true;
            await repository.SaveChangesAsync();
        }
    }

    public async Task RejectProductAsync(RejectProductFormModel model, string productId)
    {
        Guid productGuidId = Guid.Parse(productId);

        var product = await repository
            .GetByIdAsync<Product>(productGuidId);

        if (product != null && !product.IsApproved)
        {
            product.IsApproved = false;
            product.RejectionReason = model.RejectionReason;
            await repository.SaveChangesAsync();
        }
    }

}
