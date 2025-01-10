using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppGoodFriendsMVC.Models;
using Services;
using Models.DTO;

namespace AppGoodFriendsMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFriendsService _friendService;

    public HomeController(ILogger<HomeController> logger, IFriendsService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    public IActionResult Index()
    {
        var vw = new IndexViewModel();
        return View(vw);
    }

    public async Task<IActionResult> FriendsByCountry()
    {
        var vw = new FriendsByCountryViewModel() {FriendsByCountry = new Dictionary<string, int>(), CitiesByCountry = new Dictionary<string, int>()};

        GstUsrInfoAllDto dbInfo = await _friendService.InfoAsync;
        foreach (var country in dbInfo.Friends.Select(f => f.Country).Distinct())
        {
            if (string.IsNullOrEmpty(country))
            {
                var friendList = await _friendService.ReadFriendsAsync(true, false, "", 0, int.MaxValue);
                vw.FriendsByCountry["Unknown"] = friendList.PageItems.Where(f => f.Address == null).ToList().Count();
            }
            else 
            {
                vw.FriendsByCountry[country] = dbInfo.Friends
                    .Where(f => f.Country == country && !string.IsNullOrEmpty(f.City))
                    .Sum(f => f.NrFriends);
                vw.CitiesByCountry[country] = dbInfo.Friends
                    .Count(f => f.Country == country && !string.IsNullOrEmpty(f.City));
            }
        }
        return View(vw);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
