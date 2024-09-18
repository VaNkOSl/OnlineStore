namespace OnlineStore.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Controllers;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using OnlineStore.Web.ViewModels.Admin;
using OnlineStore.Web.ViewModels.Notification;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;

public class HomeController : AdminBaseController
{
    private readonly IProductService productService;
    private readonly ISellerService sellerService;
    private readonly INotificationService notificationService;
    public HomeController(IProductService _productService,ISellerService _sellerService,
                          INotificationService _notificationService)
    {
        productService = _productService;
        sellerService = _sellerService;
        notificationService = _notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> DashBoard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ProductForReview()
    {
        var model = await productService.GetUnapprovedProductsAsync();

        if(model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveProduct(string productId)
    {
        var productName = await productService.GetProductNameAsync(productId);
        await productService.ApproveProductAsync(productId);
        TempData[SuccessMessage] = string.Format(SuccessfullyApprovedAProduct, productName);  
        return RedirectToAction(nameof(ProductForReview));
    }

    [HttpGet]
    public async Task<IActionResult> RejectProduct(string productId)
    {
        var model = new RejectProductFormModel
        {
            ProductId = productId
        };
        
        if(model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RejectProduct(RejectProductFormModel model)
    {
        if (await productService.ProductExistsAsync(model.ProductId) == false)
        {
            TempData[ErrorMessage] = ProductDoesNotExists;
            return RedirectToAction("All", "Product");
        }

        if(!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            await productService.RejectProductAsync(model, model.ProductId);
            return RedirectToAction(nameof(ProductForReview));
        }
        catch (Exception)
        {
            return GeneralError();
        }

    }

    [HttpGet]
    public async Task<IActionResult> SellersForReview()
    {
        var model = await sellerService.GetUnapprovedSellersAsync();

        if(model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveSeller(string sellerId)
    {
        try
        {
            var sellerName = await sellerService.GetSellerByNameAsync(sellerId);
            await sellerService.ApproveSellerAsync(sellerId);
            TempData[SuccessMessage] = string.Format(SuccessfullyApprovedASeller, sellerName);
            return RedirectToAction(nameof(ProductForReview));
        }
        catch (Exception)
        {
            return GeneralError();
        }
       
    }

    [HttpGet]
    public async Task<IActionResult> RejectSeller(string sellerId)
    {
        var model = new RejectSellerFormModel
        {
          SellerId = sellerId
        };

        if(model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RejectSeller(RejectSellerFormModel model)
    {
        var userId = User.GetId()!;
        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(HomeController.DashBoard), "Home");
        }

        try
        {
            await sellerService.RejectSellerAsync(model, model.SellerId);
            return RedirectToAction(nameof(ProductForReview));
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Messages()
    {
        var userId = User.GetId()!;

        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(SellerController.Become), "Seller");
        }

        var sellerId = await sellerService.GetSellerByIdAsync(userId);

        if (sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }
        try
        {
            var notifications = await notificationService.GetNotificationsBySellerIdAsync(sellerId,userId,true);
            return View(notifications);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> RespondAsAdmin(string id)
    {
        try
        {
            var model = await notificationService.GetNotificationResponseModelAsync(id);

            if (model == null)
            {
                return View("~/Views/Home/Error404.cshtml");
            }

            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> RespondAsAdmin(NotificationResponseFormModel model)
    {
        var userId = User.GetId()!;

        if (await sellerService.ExistsByIdAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await notificationService.CreateResponseUserAsync(model, userId);
            TempData[SuccessMessage] = string.Format(SuccessfullySentResponse, model.UserFullName);
            return RedirectToAction(nameof(Messages));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, string.Format(UnexpectedErrorOccurredCreatingResponse, model.UserFullName));
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/Home/Error404.cshtml");
            }

            await notificationService.MarkAsRespondedByAdminAsync(id);
            return RedirectToAction(nameof(Messages));
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("DashBoard", "Home");
    }
}
