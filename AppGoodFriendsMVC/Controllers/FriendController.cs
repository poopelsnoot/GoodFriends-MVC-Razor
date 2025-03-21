using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppGoodFriendsMVC.Models;
using Models.DTO;
using Services;
using Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

    [HttpPost]
    public async Task<IActionResult> PetQuoteDelete(Guid petId, Guid quoteId, Guid friendId)
    {
        if(petId != Guid.Parse("00000000-0000-0000-0000-000000000000")) { await _friendService.DeletePetAsync(petId); } 
        if(quoteId != Guid.Parse("00000000-0000-0000-0000-000000000000")) { await _friendService.DeleteQuoteAsync(quoteId); }

        return RedirectToAction("FriendDetails", new { friendId = friendId });
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

    [HttpPost]
    public async Task<IActionResult> FriendDelete(Guid id, string city, int pageNumber)
    {
        await _friendService.DeleteFriendAsync(id);
        return RedirectToAction("ListOfFriends", new { city = city, pageNumber = pageNumber });
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
    public async Task<IActionResult> SaveFriendEdit(EditFriendViewModel vw ,List<Guid> petsId, List<Guid> quotesId)
    {
        if (!IsValid(vw))
        {
            return await EditFriend(Guid.Parse(vw.FriendToEdit.FriendId.ToString()));
        }

        if (vw.AddressToEdit.AddressId == null && vw.UserHasAddress)
        {
            var newAddress = await _friendService.CreateAddressAsync(vw.AddressToEdit);
            vw.AddressToEdit = new AddressCUdto(newAddress);
        }
        else if (vw.AddressToEdit.AddressId != null && vw.UserHasAddress)
        {
            await _friendService.UpdateAddressAsync(vw.AddressToEdit);
        }

        if(vw.UserHasAddress) { vw.FriendToEdit.AddressId = vw.AddressToEdit.AddressId; }
        
        vw.FriendToEdit.PetsId = petsId;
        vw.FriendToEdit.QuotesId = quotesId;

        await _friendService.UpdateFriendAsync(vw.FriendToEdit);
        return RedirectToAction("FriendDetails", new { friendId = vw.FriendToEdit.FriendId });
    }

    private bool IsValid(EditFriendViewModel vw, string[] validateOnlyKeys = null)
    {
        vw.InvalidKeys = ModelState.Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

        if (validateOnlyKeys != null)
        {
            vw.InvalidKeys = vw.InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
        }
        if (!vw.UserHasAddress)
        {
            vw.InvalidKeys = vw.InvalidKeys.Where(s =>
                !s.Key.StartsWith("AddressToEdit.", StringComparison.OrdinalIgnoreCase));
        }

        vw.InvalidKeys = vw.InvalidKeys.Where(s => 
            !s.Key.StartsWith("pets", StringComparison.OrdinalIgnoreCase) &&
            !s.Key.StartsWith("quotes", StringComparison.OrdinalIgnoreCase));

        vw.ValidationErrorMsgs = vw.InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
        vw.HasValidationErrors = vw.InvalidKeys.Any();

        return !vw.HasValidationErrors;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}