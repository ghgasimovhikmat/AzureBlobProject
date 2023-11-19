using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers;

public class HomeController : Controller
{
    
    private readonly IContainerService _containerService;
    private readonly IBlobService _blobService;
    public HomeController(IContainerService containerService,IBlobService blobService)
    {
        _containerService = containerService;
        _blobService = blobService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View(await _containerService.GetALlContainerAndBlobs());
    }
    public async Task<IActionResult> Images()
    {
        return View(await _blobService.GetAllBlobsWIthUri("privatecontainer"));
    }
}