namespace OnlineStore.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using OnlineStore.Web.ViewModels.Admin;
using static OnlineStore.Commons.NotificationMessagesConstants;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.EntityValidationConstraints;


public class ProductController : AdminBaseController
{
    private readonly IAdminService adminService;
    private readonly ISellerService sellerService;
    private readonly IProductService productService;

    public ProductController(IAdminService _adminService,ISellerService _sellerService, IProductService _productService)
    {
        adminService = _adminService;
        sellerService = _sellerService;
        productService = _productService;
    }

    [HttpGet]
    public async Task<IActionResult> AddColor()
    {
        var userId = User.GetId();
        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        try
        {
            return View();
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddColor(ColorFormModel model)
    {
        var userId = User.GetId();
        var adminId = await sellerService.GetSellerByIdAsync(userId);

        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        if(await adminService.ColorNameExistAsync(model.ColorName))
        {
            ModelState.AddModelError(nameof(model.ColorName), string.Format(ColorNameExists,model.ColorName));
            return View(model);
        }

        if(!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await adminService.CreateColorAsync(model, adminId);
            TempData[SuccessMessage] = string.Format(SuccessfullyAddColor, model.ColorName);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteColor(int id)
    {
        try
        {
            var model = await adminService.GetColorPreDeleteByIdAsync(id);

            if(model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteColor(int id,ColorPreDeleteViewModel model)
    {

        if (!await adminService.ColorExistByIdAsync(id))
        {
            ModelState.AddModelError(nameof(id), ColorIdDoesNotExists);
            return await LoadColorsForView(id);
        }

        try
        {
            await adminService.DeleteColorAsync(id);
            TempData[WarningMessage] = string.Format(SuccessfullyDeleteColor, model.Name);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddBrand()
    {
        var userId = User.GetId();
        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        try
        {
            return View();
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddBrand(BrandFormModel model)
    {
        var userId = User.GetId();
        var adminId = await sellerService.GetSellerByIdAsync(userId);

        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        if(await adminService.BrandNameExistAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name),string.Format(BrandNameExists,model.Name));
            return View(model);
        }

        if(!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await adminService.CreateBrandAsync(model, adminId);
            TempData[SuccessMessage] = string.Format(SuccessfullyAddBrand,model.Name);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        try
        {
            var model = await adminService.GetBrandPreDeleteByIdAsync(id);

            if(model == null)
            {
                return NotFound();
            }

            return View(model); 
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBrand(int id,BrandPreDeleteViewModel model)
    {

        if (await adminService.BrandExistByIdAsync(id) == false)
        {
            return await LoadBrandView(id);
        }

        try
        {
            await adminService.DeleteBrandAsync(id);
            TempData[WarningMessage] = string.Format(SuccessfullyDeleteBrand,model.Name);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddSize()
    {
        var userId = User.GetId();
        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        try
        {
            return View();
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddSize(SizeFormModel model)
    {
        var userId = User.GetId();
        var adminId = await sellerService.GetSellerByIdAsync(userId);

        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        if(await adminService.SizeNameExistAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name), string.Format(SizeNameExists,model.Name));
            return View(model);
        }

        try
        {
            await adminService.CreateSizeAsync(model, adminId);
            TempData[SuccessMessage] = string.Format(SuccessfullyAddSize, model.Name);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteSize(int id)
    {
        try
        {
            var model = await adminService.GetSizePreDeleteByIdAsync(id);

            if(model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSize(int id,SizePreDeleteViewModel model)
    {
        if(await adminService.SizeExistByIdAsync(id) == false)
        {
            ModelState.AddModelError(nameof(id), SizeWithIdDoesNotExists);
            return View(model);
        }

        try
        {
            await adminService.DeleteSizeAsync(id);
            TempData[WarningMessage] = string.Format(SuccessfullyDeleteSize,model.Name);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddCategory()
    {
        var userId = User.GetId();
        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        try
        {
            return View();
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryFormModel model)
    {
        var userId = User.GetId();
        var adminId = await sellerService.GetSellerByIdAsync(userId);
        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        if(await adminService.CategoryNameExistAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name),string.Format(CategoryNameExists,model.Name));
            return View(model);
        }

        try
        {
            await adminService.CreateCategoryAsync(model, adminId);
            TempData[SuccessMessage] = string.Format(SuccessfullyAddCategory, model.Name);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var model = await adminService.GetCategoryPreDeleteByIdAsync(id);

            if(model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id,CategoryPreDeleteViewModel model)
    {
        if(await adminService.CategoryExistByIdAsync(id) == false)
        {
            ModelState.AddModelError(nameof(id), CategoryWithIdDoesNotExists);
            return View(model);
        }

        try
        {
            await adminService.DeleteCategoryAsync(id);
            TempData[WarningMessage] = string.Format(SuccessfullyDeleteCategory,model.Name);
            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Mine()
    {
        var userId = User.GetId()!;
        var sellerId = await sellerService.GetSellerByIdAsync(userId);

        var model = new MyProductViewModel
        {
            AddedProduct = await productService.AllProductsBySellerIdAsync(sellerId),
        };

        return View(model);
    }

    private async Task<IActionResult> LoadBrandView(int brandId)
    {
        var brands = await productService.AllBrandsAsync();
        var model = brands.Select(b => new BrandPreDeleteViewModel
        {
           Id = b.Id,
           Name = b.Name,
           ImageUrl = b.ImageUrl,
        });

        return View(model);
    }
    private async Task<IActionResult> LoadColorsForView(int colorId)
    {
        var colors = await productService.AllColorsAsync();
        var model = colors.Select(c => new ColorPreDeleteViewModel
        {
            Id = c.ColorId,
            Name = c.ColorName
        }).ToList();

        return View(model);
    }
    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("DashBoard", "Home");
    }
}
