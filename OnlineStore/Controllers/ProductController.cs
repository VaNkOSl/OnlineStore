namespace OnlineStore.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using OnlineStore.Web.ViewModels.Products;
using OnlineStore.Web.ViewModels.Reviews;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;
using static OnlineStore.Commons.GeneralApplicationConstants;
public class ProductController : BaseController
{
    private readonly IProductService productService;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly ISellerService sellerService;
    private readonly IProductAttributeService productAttributeService;

    public ProductController(IProductService _productService, IWebHostEnvironment _webHostEnvironment,
                             ISellerService _sellerService, IProductAttributeService _productAttributeService)
    {
        productService = _productService;
        webHostEnvironment = _webHostEnvironment;
        sellerService = _sellerService;
        productAttributeService = _productAttributeService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All([FromQuery] AllProductsQueryModel model)
    {
        ProductQueryServiceModel serviceModel = await productService.AllProductsAsync(model);

        model.TotalProducs = serviceModel.TotalProductsCount;
        model.Products = serviceModel.Products;
        model.Categories = await productAttributeService.AllCategoriesNamesAsync();
        model.Brands = await productAttributeService.AllBrandsNameAsync();
        model.Colors = await productAttributeService.AllColorsNameAsync();
        model.Sizes = await productAttributeService.AllSizesNameAsync();

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddReview(ProductReviewViewModel model)
    {
        var productName = await productService.GetProductNameAsync(model.ProductId);

        try
        {
            var userId = User.GetId()!;
            var review = await productService.CreateProductReviewAsync(model, userId);
            TempData[SuccessMessage] =string.Format(ReviewSuccessfullyAdded,productName);
        }
        catch (Exception)
        {                                         
            ModelState.AddModelError(string.Empty, string.Format(ErrorWhileCreatingAReview, productName));
            throw;
        }

        var productModel = await productService.GetProductDetailsByIdAsync(model.ProductId);

        if (productModel == null)
        {
            TempData[ErrorMessage] = ProductNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        return View(nameof(Details), productModel);
    }

    [HttpGet]
    public async Task<IActionResult> Mine()
    {
        IEnumerable<ProductServiceModel>? myProducts = null;

     
        var userId = User.GetId()!;
        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(SellerController.Become), "Seller");
        }

        if (User.IsInRole(AdminRoleName))
        {
            return RedirectToAction("Mine", "Product", new { area = AdminAreaName });
        }

        try
        {
            var currentSellerId = await sellerService.GetSellerByIdAsync(userId);

            if (currentSellerId != null)
            {
                myProducts = await productService.AllProductsBySellerIdAsync(currentSellerId);
            }
        }
        catch (Exception)
        {
            return GeneralError();
        }

        return View(myProducts);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var userId = User.GetId()!;

        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(SellerController.Become), "Seller");
        }

        try
        {
            ProductFormModel model = new ProductFormModel
            {
                Categories = await productService.AllCategoriesAsync(),
                Brands = await productService.AllBrandsAsync(),
                Colors = await productService.AllColorsAsync(),
                Sizes = await productService.AllSizesAsync(),
            };

            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add(ProductFormModel model, List<int> selectedColorIds, List<int> selectedSizeIds,IFormFileCollection images)
    {
        var userId = User.GetId()!;
        var sellerId = await sellerService.GetSellerByIdAsync(userId);

        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(SellerController.Become), "Seller");
        }

        if (sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        if (await productService.SizeExistsAsync(selectedSizeIds) == false)
        {
            ModelState.AddModelError(nameof(selectedSizeIds), SizeDoesNotExists);
            return View(model);
        }

        if(await productService.ColorExistsAsync(selectedColorIds) == false)
        {
            ModelState.AddModelError(nameof(selectedColorIds), ColorDoesNotExists);
            return View(model);
        }

        if(await productService.CategoryExistsAsync(model.CategoryId) == false)
        {
            ModelState.AddModelError(nameof(model.CategoryId), CategoryDoesNotExists);
            return View(model);
        }

        if(await productService.BrandExistsAsync(model.BrandId) == false)
        {
            ModelState.AddModelError(nameof(model.BrandId), BrandDoesNotExists);
            return View(model);
        }

        if (images.Count <= 0 || !images.Any())
        {
            TempData[ErrorMessage] = ImageDoesNotSelected;

            await LoadModelDataAsync(model);
            return View(model);
        }

        if(selectedColorIds == null || !selectedColorIds.Any())
        {
            TempData[ErrorMessage] = ColorDoesNotSelected;

            await LoadModelDataAsync(model);
            return View(model);
        }

        if(selectedSizeIds == null || !selectedSizeIds.Any())
        {
            TempData[ErrorMessage] = SizeDoesNotSelected;

            await LoadModelDataAsync(model);
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            await LoadModelDataAsync(model);
            return View(model);
        }

        try
        {
            var product = await productService.CreateProductAsync(model, sellerId, selectedColorIds, selectedSizeIds, images);
            TempData[SuccessMessage] = string.Format(SuccessfullyCreatedAProduct, model.Name);

            return RedirectToAction(nameof(Details), new { id = product });
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, ErrorWhileCreatedProduct);
            return RedirectToAction("All", "Product");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var productName = await productService.GetProductNameAsync(id);

        if (await productService.ProductExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = string.Format(ProductDoesNotExists,productName);
            return View("~/Views/Home/Error404.cshtml");
        }

        try
        {
            var productModel = await productService.GetProductDetailsByIdAsync(id);

            if (productModel == null)
            {
                TempData[ErrorMessage] = ProductNotFound;
                return View("~/Views/Home/Error404.cshtml");
            }

            return View(productModel);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        string sellerId = await sellerService.GetSellerByIdAsync(User.GetId()!);

        if(sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        if(await productService.ProductExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = ProductNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        try
        {
            ProductPreDeleteViewModel model = await productService
                .GetProductForDeletingByIdAsync(id);

            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(ProductPreDeleteViewModel model ,string id)
    {
        string sellerId = await sellerService.GetSellerByIdAsync(User.GetId()!);

        if (sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        if (await productService.ProductExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = ProductNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (await productService.ProductHasOrdersAsync(id) == true)
        {
            TempData[ErrorMessage] = ProductHasOrders;
            return RedirectToAction("All", "Product");
        }

        try
        {
            await productService.DeleteProductByIdAsync(id);
            TempData[WarningMessage] = string.Format(SuccessfullyDeleteProduct, model.Name);
            return RedirectToAction("All", "Product");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, string.Format(ErrorWhileDeleteAProduct,model.Name));
            return RedirectToAction("All", "Product");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var productName = await productService.GetProductNameAsync(id);

        if (await productService.ProductExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = string.Format(ProductDoesNotExists,productName);
            return View("~/Views/Home/Error404.cshtml");
        }

        string sellerId = await sellerService.GetSellerByIdAsync(User.GetId()!);

        if (sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return RedirectToAction(nameof(SellerController.Become), "Seller");
        }

        try
        {
            var productToEdit = await productService.GetProductForEditByIdAsync(id);
            return View(productToEdit);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductFormModel model,string id, List<int> selectedColorIds, List<int> selectedSizeIds, IFormFileCollection images)
    {
        if (await productService.SizeExistsAsync(selectedSizeIds) == false)
        {
            ModelState.AddModelError(nameof(selectedSizeIds), SizeDoesNotExists);
        }

        if (await productService.ColorExistsAsync(selectedColorIds) == false)
        {
            ModelState.AddModelError(nameof(selectedColorIds), ColorDoesNotExists);
        }

        if (await productService.CategoryExistsAsync(model.CategoryId) == false)
        {
            ModelState.AddModelError(nameof(model.CategoryId), CategoryDoesNotExists);
        }

        if (await productService.BrandExistsAsync(model.BrandId) == false)
        {
            ModelState.AddModelError(nameof(model.BrandId), BrandDoesNotExists);
        }

        if (!ModelState.IsValid)
        {
            await LoadModelDataAsync(model);
            return View(model);
        }

        try
        {
            await productService.EditProductByFormModelAsync(model, id, selectedColorIds,selectedSizeIds, images);
        
            TempData[SuccessMessage] = string.Format(SuccessfullyEditProduct, model.Name);
            return RedirectToAction(nameof(Details), new { id });

        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, string.Format(ErrorWhileEditProduct,model.Name));
            return RedirectToAction("All", "Product");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ApplyDiscount(string id)
    {
        var product = await productService.GetProductByIdAsync(id);
        var userId = User.GetId();

        string sellerId = await sellerService.GetSellerByIdAsync(userId!);

        if (sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        if(await productService.ProductExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = ProductNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (product == null)
        {
            TempData[ErrorMessage] = ProductNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        try
        {
            return View(new ApplyDiscountViewModel { Id = id, Name = product.Name });
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> ApplyDiscount(ApplyDiscountViewModel model)
    {
        var product = await productService.GetProductByIdAsync(model.Id);
        var userId = User.GetId();

        string sellerId = await sellerService.GetSellerByIdAsync(userId!);

        if (sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        if (product == null)
        {
            TempData[ErrorMessage] = ProductNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (await productService.ProductExistsAsync(model.Id) == false)
        {
            TempData[ErrorMessage] = ProductNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await productService.ApplyDiscountAsync(model.Id, model.DiscountPercentage);

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, GeneralErrors);
            return RedirectToAction("All", "Product");
        }


    }
    private async Task LoadModelDataAsync(ProductFormModel model)
    {
        model.Categories = await productService.AllCategoriesAsync();
        model.Brands = await productService.AllBrandsAsync();
        model.Colors = await productService.AllColorsAsync();
        model.Sizes = await productService.AllSizesAsync();
    }
    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("Index", "Home");
    }
}
