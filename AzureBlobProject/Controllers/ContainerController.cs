using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers;

public class ContainerController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}