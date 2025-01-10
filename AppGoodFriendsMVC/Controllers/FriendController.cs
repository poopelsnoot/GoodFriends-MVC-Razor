using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppGoodFriendsMVC.Models;

namespace AppGoodFriendsMVC.Controllers;

public class FriendController : Controller
{
    private readonly ILogger<FriendController> _logger;

    public FriendController(ILogger<FriendController> logger)
    {
        _logger = logger;
    }

    public IActionResult FriendsByCity()
    {
        var vw = new FriendsByCityViewModel();
        return View(vw);
    }

    public IActionResult FriendDetails()
    {
        var vw = new FriendDetailsViewModel();
        return View(vw);
    }

    public IActionResult ListOfFriends()
    {
        var vw = new ListOfFriendsViewModel();
        return View(vw);
    }

    public IActionResult EditFriend()
    {
        var vw = new EditFriendViewModel();
        return View(vw);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}