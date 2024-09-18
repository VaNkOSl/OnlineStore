namespace OnlineStore.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using OnlineStore.Web.ViewModels.Sellers;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;

public class SellerController : BaseController
{
    private readonly ISellerService sellerService;
    private readonly IUserService userService;

    public SellerController(ISellerService _sellerService, IUserService _userService)
    {
        sellerService = _sellerService;
        userService = _userService;
    }

    [HttpGet]
    public async Task<IActionResult> Become()
    {
        var userId = User.GetId()!;

        if(await sellerService.IsAdminRejectedAsync(userId) == true)
        {
            TempData[ErrorMessage] = AdminIsRejected;
            return RedirectToAction("UserNotifications", "Notification");
        }

        var userFullName = await userService.GetUserFullNameAsync(userId);

        if(await sellerService.ExistsByIdAsync(userId) == true)
        {
            TempData[ErrorMessage] = string.Format(UserIsAlreadySeller, userFullName);
            return RedirectToAction("All", "Product");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Become(SellerFormModel model)
    {
        var userId = User.GetId()!;
        var userFullName = await userService.GetUserFullNameAsync(userId);

        if (await sellerService.ExistsByIdAsync(userId) == true)
        {
            TempData[ErrorMessage] = string.Format(UserIsAlreadySeller, userFullName);
            return RedirectToAction("All", "Product");
        }

        if (await sellerService.SellerWithEgnAlredyExistsAsync(model.Egn) == true)
        {
            ModelState.AddModelError(nameof(model.Egn), UserWithTheSameEgnExists);
            return View(model);
        }

        if (await sellerService.SellerWithPhoneNumberAlredyExistsAsync(model.PhoneNumber) == true)
        {
            ModelState.AddModelError(nameof(model.PhoneNumber), UserWithPhoneNumberExists);
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            TempData[ErrorMessage] = InvalidModel;
            return View(model);
        }

        try
        {
            await sellerService.CreateSellerAsync(model, userId);
            TempData[SuccessMessage] = ApplicationSuccessfully;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = UnexpectedErrorOccurredCreatingSeller;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
