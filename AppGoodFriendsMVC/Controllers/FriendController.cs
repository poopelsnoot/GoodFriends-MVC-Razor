using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppGoodFriendsMVC.Models;
using Models.DTO;
using Services;
using Models;

namespace AppGoodFriendsMVC.Controllers;

public class FriendController : Controller
{
    private readonly ILogger<FriendController> _logger;
    private readonly IFriendsService _friendService;

    public FriendController(ILogger<FriendController> logger, IFriendsService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    [HttpGet]
    public async Task<IActionResult> FriendsByCity(string country)
    {
        var vw = new FriendsByCityViewModel();

        GstUsrInfoAllDto dbInfo = await _friendService.InfoAsync;
        vw.ChosenCountry = country;

        if (country != "Unknown") 
        {
            var listAddressesInCountry = await _friendService.ReadAddressesAsync(true, false, country, 0, int.MaxValue);
            var unseededAddressesInCountry = await _friendService.ReadAddressesAsync(false, false, country, 0, int.MaxValue);
            listAddressesInCountry.PageItems.AddRange(unseededAddressesInCountry.PageItems);

            var listCitiesInCountry = listAddressesInCountry.PageItems.Where(f => f.Country == country).Select(f => f.City).Distinct().ToList();
            foreach (var city in listCitiesInCountry)
            {
                vw.FriendsByCity[city] = dbInfo.Friends
                    .Where(f => f.City == city)
                    .Sum(f => f.NrFriends);
                vw.PetsByCity[city] = dbInfo.Pets
                    .Where(f => f.City == city)
                    .Sum(f => f.NrPets);
            }
        }
        else 
        { 
            var friendList = await _friendService.ReadFriendsAsync(true, false, "", 0, int.MaxValue);
            vw.FriendsByCity["Unknown"] = friendList.PageItems
                .Where(f => f.Address == null)
                .ToList().Count();
            vw.PetsByCity["Unknown"] = friendList.PageItems
                .Where(f => f.Address == null)
                .SelectMany(f => f.Pets)
                .ToList()
                .Count();
        }

        return View(vw);
    }

    [HttpGet]
    public async Task<IActionResult> FriendDetails(Guid friendId)
    {
        var vw = new FriendDetailsViewModel();

        vw.Friend = await _friendService.ReadFriendAsync(friendId, false);

        return View(vw);
    }

    [HttpDelete]
    public async Task<IActionResult> PetQuoteDelete(Guid petId, Guid quoteId, Guid friendId)
    {
        if(petId != Guid.Parse("00000000-0000-0000-0000-000000000000")) { await _friendService.DeletePetAsync(petId); } 
        if(quoteId != Guid.Parse("00000000-0000-0000-0000-000000000000")) { await _friendService.DeleteQuoteAsync(quoteId); }

        return await FriendDetails(friendId);
    }

    [HttpGet]
    public async Task<IActionResult> ListOfFriends(string city, int pageNumber = 1)
    {
        var vw = new ListOfFriendsViewModel();

        vw.ChosenCity = city;
        vw.CurrentPage = pageNumber;
        List<IFriend> AllFriendsInCity = new List<IFriend>();

        if (vw.ChosenCity != "Unknown")
        {
            var AddressesInCity = await _friendService.ReadAddressesAsync(true, false, vw.ChosenCity, 0, int.MaxValue);
            var unseededAddressesInCity = await _friendService.ReadAddressesAsync(false, false, vw.ChosenCity, 0, int.MaxValue);
            AddressesInCity.PageItems.AddRange(unseededAddressesInCity.PageItems);

            AllFriendsInCity = AddressesInCity.PageItems.SelectMany(a => a.Friends).ToList();
        }
        else
        {
            var allFriends = await _friendService.ReadFriendsAsync(true, false, "", 0, int.MaxValue);
            var unseededFriends = await _friendService.ReadFriendsAsync(false, false, "", 0, int.MaxValue);
            allFriends.PageItems.AddRange(unseededFriends.PageItems);
            
            AllFriendsInCity = allFriends.PageItems.Where(f => f.Address == null).ToList();
        }

        vw.TotalPages = (int)Math.Ceiling((double)AllFriendsInCity.Count() / 10);
        vw.FriendsList = AllFriendsInCity.Skip((vw.CurrentPage-1) * 10).Take(10).ToList();

        return View(vw);
    }

    [HttpDelete]
    public async Task<IActionResult> FriendDelete(Guid id, string city, int pageNumber)
    {
        await _friendService.DeleteFriendAsync(id);
        return await ListOfFriends(city, pageNumber);
    }

    [HttpGet]
    public async Task<IActionResult> EditFriend(Guid friendId)
    {
        var vw = new EditFriendViewModel();

        try
        {
            var Friend = await _friendService.ReadFriendAsync(friendId, false);
            vw.FriendToEdit = new FriendCUdto(Friend) {PetsId = Friend.Pets.Select(p => p.PetId).ToList(), QuotesId = Friend.Quotes.Select(q => q.QuoteId).ToList()};

            if(Friend.Address != null)
            {
                vw.AddressToEdit = new AddressCUdto(Friend.Address);
            }
            else
            {
                vw.AddressToEdit = new AddressCUdto();
            }

            vw.UserHasAddress = !string.IsNullOrEmpty(Friend.Address?.StreetAddress);
        }
        catch (Exception e)
        {
            vw.ErrorMessage = e.Message;
        }

        return View(vw);
    }

    [HttpPost]
    public async Task<IActionResult> SaveFriendEdit(List<Guid> petsId, List<Guid> quotesId)
    {
        if (!IsValid())
        {
            return Page();
        }

        if (AddressToEdit.AddressId == null && UserHasAddress)
        {
            var newAddress = await _service.CreateAddressAsync(AddressToEdit);
            AddressToEdit = new AddressCUdto(newAddress);
        }
        else if (AddressToEdit.AddressId != null && UserHasAddress)
        {
            await _service.UpdateAddressAsync(AddressToEdit);
        }

        if(UserHasAddress) { FriendToEdit.AddressId = AddressToEdit.AddressId; }
        
        FriendToEdit.PetsId = petsId;
        FriendToEdit.QuotesId = quotesId;

        await _service.UpdateFriendAsync(FriendToEdit);
        return await ListOfFriends(city, pageNumber);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}