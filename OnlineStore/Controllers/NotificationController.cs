namespace OnlineStore.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using OnlineStore.Web.ViewModels.Notification;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;
using static OnlineStore.Commons.GeneralApplicationConstants;

[Authorize]
public class NotificationController : BaseController
{
    private readonly INotificationService notificationService;
    private readonly ISellerService sellerService;
    private readonly IUserService userService;
    public NotificationController(INotificationService _notificationService, 
                               ISellerService _sellerService,IUserService _userService)
    {
        notificationService = _notificationService;
        sellerService = _sellerService;
        userService = _userService;
    }

    [HttpGet]
    public async Task<IActionResult> CreateMessege()
    {
        try
        {
            NotificationFormModel model = new NotificationFormModel();

            if (model == null)
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
    public async Task<IActionResult> CreateMessege(NotificationFormModel model)
    {
        var userId = User.GetId()!;

        if(await userService.UserExistsAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (await sellerService.SellerEmailExistsAsync(userId, model.Email) == false)
        {
            ModelState.AddModelError(nameof(model.Email), SellerWithEmailNotExists);
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await notificationService.CreateMessegeAsync(model, userId);
            TempData[SuccessMessage] = string.Format(SuccessfullySentAMessage,model.Email);
            return RedirectToAction("UserNotifications", "Notification");
        }
        catch(Exception)
        {
            ModelState.AddModelError(string.Empty, string.Format(UnexpectedErrorOccurredCreatingMessege, model.Email));
            return RedirectToAction(nameof(ProductController.All), "Product");
        }
    }

    [HttpGet]
    public async Task<IActionResult> UserNotifications()
    {
        try
        {
            var userId = User.GetId()!;

            if (await userService.UserExistsAsync(userId) == false)
            {
                TempData[ErrorMessage] = UserNotFound;
                return View("~/Views/Home/Error404.cshtml");
            }

            if (await sellerService.ExistsByIdAsync(userId) == true)
            {
                return RedirectToAction("NotificationsForSeller", "Notification");
            }

            var notifications = await notificationService.GetNotificationsByUserIdAsync(userId);
            return View(notifications);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> NotificationsForSeller()
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
            return View("~/Views/Home/Error401.cshtml");
        }

        try
        {
            var notifications = await notificationService.GetNotificationsBySellerIdAsync(sellerId, userId, false);

            return View(notifications);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Respond(string id)
    {
        try
        {
            var model = await notificationService.GetNotificationResponseModelAsync(id);

            if (model == null)
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
    public async Task<IActionResult> Respond(NotificationResponseFormModel  model)
    {
        var userId = User.GetId()!;

        if (await userService.UserExistsAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await notificationService.CreateResponseUserAsync(model, userId);
            TempData[SuccessMessage] = string.Format(SuccessfullySentResponse,model.UserFullName);
            return RedirectToAction(nameof(NotificationsForSeller));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, string.Format(UnexpectedErrorOccurredCreatingResponse, model.UserFullName));
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> UserRespond(string id)
    {
        try
        {
            var model = await notificationService.GetNotificationResponseModelAsync(id);

            if (model == null)
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
    public async Task<IActionResult> UserRespond(NotificationResponseFormModel model)
    {
        var userId = User.GetId()!;

        if (await userService.UserExistsAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await notificationService.CreateResponseSellerAsync(model, userId);
            TempData[SuccessMessage] = string.Format(SuccessfullySentResponse, model.UserFullName);
            return RedirectToAction(nameof(UserNotifications));
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
            if(string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/Home/Error404.cshtml");
            }

            if(User.IsInRole(AdminRoleName))
            {
                return RedirectToAction("Messages", "Home", new { area = AdminAreaName });
            }

            await notificationService.MarkAsRespondedBySellerAsync(id);
            return RedirectToAction(nameof(NotificationsForSeller));
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsReadByUser(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/Home/Error404.cshtml");
            }

            await notificationService.MarkAsRespondedByUserAsync(id);
            return RedirectToAction(nameof(UserNotifications));
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSellerRequest(string sellerId)
    {
        string seller = await sellerService.GetSellerByIdAsync(User.GetId()!);

        if (seller == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        try
        {
            await notificationService.DeleteSellerRequestAsync(sellerId);
            return RedirectToAction(nameof(UserNotifications));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, UnexpectedErrorOccurredDeletingSellerRequest);
            return RedirectToAction("UserNotifications", "Notification");
        }
    }

    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("Index", "Home");
    }
}
