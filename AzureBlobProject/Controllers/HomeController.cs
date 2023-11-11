using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers;

public class HomeController : Controller
{
    
    private readonly IContainerService _containerService;

    public HomeController(IContainerService containerService)
    {
        _containerService = containerService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View(await _containerService.GetALlContainerAndBlobs());
    }
    
}