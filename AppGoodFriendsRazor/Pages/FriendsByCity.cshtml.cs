using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Models.DTO;

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

            return Page();
        }

        public FriendsByCityModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
