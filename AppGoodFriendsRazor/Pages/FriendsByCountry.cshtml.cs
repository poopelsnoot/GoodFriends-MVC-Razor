using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace MyApp.Namespace
{
    public class FriendsByCountryModel : PageModel
    {
        readonly IFriendsService _service;
        public Dictionary<string, int> FriendsByCountry { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CitiesByCountry { get; set; } = new Dictionary<string, int>();

        public async Task<IActionResult> OnGet()
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;

            foreach (var country in dbInfo.Friends.Select(f => f.Country).Distinct())
            {
                if (string.IsNullOrEmpty(country))
                {
                    var friendList = await _service.ReadFriendsAsync(true, false, "", 0, int.MaxValue);
                    FriendsByCountry["Unknown"] = friendList.PageItems.Where(f => f.Address == null).ToList().Count();
                }
                else 
                {
                    FriendsByCountry[country] = dbInfo.Friends
                        .Where(f => f.Country == country && !string.IsNullOrEmpty(f.City))
                        .Sum(f => f.NrFriends);
                    CitiesByCountry[country] = dbInfo.Friends
                        .Count(f => f.Country == country && !string.IsNullOrEmpty(f.City));
                }
            }

            return Page();
        }

        public FriendsByCountryModel(IFriendsService service)
        {
            _service = service;
        }

    }
}
