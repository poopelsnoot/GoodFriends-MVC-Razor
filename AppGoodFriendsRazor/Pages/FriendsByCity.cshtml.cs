using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Models.DTO;
using Models;

namespace MyApp.Namespace
{
    public class FriendsByCityModel : PageModel
    {
        readonly IFriendsService _service;
        public string ChosenCountry { get; set; }
        public Dictionary<string, int> FriendsByCity { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PetsByCity { get; set; } = new Dictionary<string, int>();

        public async Task<IActionResult> OnGet(string country)
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;
            ChosenCountry = country;

            if (country != "Unknown") 
            {
                var listAddressesInCountry = await _service.ReadAddressesAsync(true, false, country, 0, int.MaxValue);
                var listCitiesInCountry = listAddressesInCountry.PageItems.Where(f => f.Country == country).Select(f => f.City).Distinct().ToList();
                foreach (var city in listCitiesInCountry)
                {
                    FriendsByCity[city] = dbInfo.Friends
                        .Where(f => f.City == city)
                        .Sum(f => f.NrFriends);
                    PetsByCity[city] = dbInfo.Pets
                        .Where(f => f.City == city)
                        .Sum(f => f.NrPets);
                }
            }
            else 
            { 
                var friendList = await _service.ReadFriendsAsync(true, false, "", 0, int.MaxValue);
                FriendsByCity["Unknown"] = friendList.PageItems
                    .Where(f => f.Address == null)
                    .ToList().Count();
                PetsByCity["Unknown"] = friendList.PageItems
                    .Where(f => f.Address == null)
                    .SelectMany(f => f.Pets)
                    .ToList()
                    .Count();
            }

            return Page();
        }

        public FriendsByCityModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
