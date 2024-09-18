namespace OnlineStore.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Admin;
using static OnlineStore.Commons.GeneralApplicationConstants;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;

public class UserController : AdminBaseController
{
    private readonly IUserService userService;
    private readonly IMemoryCache memoryCache;

    public UserController(IUserService _userService,IMemoryCache _memoryCache)
    {
        userService = _userService;
        memoryCache = _memoryCache;
    }

    [Route("User/All")]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client,NoStore = false)]
    public async Task<IActionResult> All()
    {
        IEnumerable<UserViewModel> users = this.memoryCache.Get<IEnumerable<UserViewModel>>(UsersCacheKey);

        if(users == null)
        {
            users = await userService.GetAllUsersAsync();

            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(UsersCacheDurationMinutes));

            memoryCache.Set(UsersCacheKey, users,cacheOptions);
        }

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> BlockUser(string userId)
    {
        if(await userService.UserExistsAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        try
        {
            var model = await userService.GetUserToBlockAsync(userId);

            if (!model.Any())
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
    public async Task<IActionResult> BlockUser(string userId,BlockUserViewModel model)
    {
        if(await userService.UserExistsAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return RedirectToAction("All","User");
        }

        try
        {
            await userService.BlockUserAsync(userId);
            TempData[WarningMessage] = string.Format(SuccessfullyBlockAUser, model.UserFullName);
            return RedirectToAction("All", "User");
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = string.Format(UnexpectedErrorWhileBlockinUser, model.UserFullName);
            return RedirectToAction("DashBoard", "Home");
        }
    }

    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralAdminError;

        return RedirectToAction("DashBoard", "Home", new { Area = "Admin" });
    }
}
