using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppGoodFriendsMVC.Models;
using Services;
using System.Net.WebSockets;

namespace AppGoodFriendsMVC.Controllers;

public class HomeController : Controller
{
    readonly ILogger<HomeController> _logger;
    readonly IQuoteService _service = null;


    public HomeController(ILogger<HomeController> logger, IQuoteService service)
    {
        _service = service;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("private")]
    [Route("home/privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Routes()
    {
        return View();
    }







    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

