namespace OnlineStore.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using OnlineStore.Web.ViewModels.Orders;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;

[Authorize]
public class OrderController : BaseController
{
    private readonly IOrderService orderService;
    private readonly IProductService productService;
    private readonly ISellerService sellerService;
    private readonly IShoppingCartService shoppingCartService;
    private readonly IProductAttributeService productAttributeService;
    private readonly IUserService userService;

    public OrderController(IOrderService _orderService, IProductService _productService, 
                          IProductAttributeService _productAttributeService, ISellerService _sellerService,
                          IShoppingCartService _shoppingCartService, IUserService _userService)
    {
        orderService = _orderService;
        productService = _productService;
        productAttributeService = _productAttributeService;
        sellerService = _sellerService;
        shoppingCartService = _shoppingCartService;
        userService = _userService;
    }


    [HttpGet]
    public async Task<IActionResult> MyOrder(string id)
    {
        var userId = User.GetId()!;

        if (await userService.UserExistsAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        try
        {
            var orders = await orderService.GetOrderByUserIdAsync(userId);

            return View(orders);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> CompleteOrder()
    {
        if (await userService.UserExistsAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if (await shoppingCartService.UserHasItemsInCart(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = UserHasNoItemInCartItem;
            return RedirectToAction(nameof(ProductController.All), "Product");
        }

        try
        {
            var orderFormModel = new OrderFormModel();

            return View(orderFormModel);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CompleteOrder(OrderFormModel model)
    {
        var userId = User.GetId()!;

        if (await userService.UserExistsAsync(userId) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        if(await shoppingCartService.UserHasItemsInCart(userId) == false)
        {
            TempData[ErrorMessage] = UserHasNoItemInCartItem;
            return RedirectToAction(nameof(ProductController.All), "Product");
        }

        if (!ModelState.IsValid)
        {
            TempData[ErrorMessage] = InvalidModel;
            return View(model);
        }

        try
        {
            var orderId =  await orderService.CreateOrderAsync(model, userId);
            await shoppingCartService.ClearCartItemsAsync();
            TempData[SuccessMessage] = SuccessfullyCompletedOrder;
            return RedirectToAction(nameof(MyOrder), new { id = orderId });
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = UnexpectedErrorOccurredCompleteOrder;
            return RedirectToAction("CartItem", "ShoppingCart");
        }
    }

    [HttpGet]
    public async Task<IActionResult> OrdersForProduct()
    {
        if (await sellerService.ExistsByIdAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = UserNotASeller;
            return RedirectToAction(nameof(SellerController.Become), "Seller");
        }

        var sellerId = await sellerService.GetSellerByIdAsync(User.GetId()!);

        if (sellerId == null)
        {
            TempData[ErrorMessage] = SellerNotFound;
            return View("~/Views/Home/Error401.cshtml");
        }

        try
        {
            var orders = await orderService.GetOrdersByProductAndSellerAsync(sellerId);

            return View(orders);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendOrder(string orderId)
    {
        if (await orderService.OrderExistsAsync(orderId) == false)
        {
            TempData[ErrorMessage] = OrderNotFound;
            return RedirectToAction("All", "Product");
        }

        try
        {
            bool result = await orderService.SendOrderAsync(orderId);

            if (!result)
            {
                TempData[ErrorMessage] = OrderNotFound;
                return RedirectToAction("OrdersForProduct", "Order");
            }

            TempData[SuccessMessage] = SuccessfullySendOrder;
            return RedirectToAction("OrdersForProduct", "Order");
        }
        catch(Exception)
        {
            TempData[ErrorMessage] = UnexpectedErrorOccurredSendOrder;
            return RedirectToAction("OrdersForProduct", "Order");
        }
    }

    [HttpPost]
    public async Task <IActionResult> TakeOrder(string orderId)
    {
        if(await orderService.OrderExistsAsync(orderId) == false)
        {
            TempData[ErrorMessage] = OrderNotFound;
            return RedirectToAction("All", "Product");
        }

        if (await userService.UserExistsAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = UserNotFound;
            return View("~/Views/Home/Error404.cshtml");
        }

        try
        {
            await orderService.TakeOrderAsync(orderId);
            TempData[SuccessMessage] = SuccessfullyTakeOrder;
            return RedirectToAction("MyOrder", "Order");
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = UnexpectedErrorOccurredTakeOrder;
            return RedirectToAction("MyOrder", "Order");
        }
    }

    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("Index", "Home");
    }
}
