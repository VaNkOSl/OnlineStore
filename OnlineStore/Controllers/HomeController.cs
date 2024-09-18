namespace OnlineStore.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Products;
using static OnlineStore.Commons.GeneralApplicationConstants;

public class HomeController : BaseController
{
    private readonly IProductService productService;

    public HomeController(IProductService _productService)
    {
        productService = _productService; 
    }
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(AdminRoleName))
        {
            return RedirectToAction("DashBoard", "Home", new { area = AdminAreaName });
        }

        IEnumerable<ProductIndexServiceModel> products = await productService.LastProductsAsync();

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> AboutUs()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int statusCode)
    {
        if (statusCode == 400 || statusCode == 404)
        {
            return View("Error404");
        }

        if (statusCode == 401)
        {
            return View("Error401");
        }

        return View();
    }
}
