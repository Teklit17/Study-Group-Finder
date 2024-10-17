using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SG_Finder.Models;

namespace SG_Finder.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
  
        public IActionResult Messages()
        {
            return RedirectToAction("Index", "Messages");
        }
    

    public IActionResult Notification()
    {
        return RedirectToAction("Index", "Notification");
    }
    
    
    public IActionResult Calendar()
    {
        return View();
    }
    public IActionResult Profile()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }
    
    public IActionResult StudygroupFinder    ()
    {
        return View();
    }

    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}