using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace MyApp.Namespace
{
    public class ListOfFriendsModel : PageModel
    {
        readonly IFriendsService _service;
        public List<IFriend> FriendsList { get; set; }
        public string ChosenCity { get; set; }
        public async Task<IActionResult> OnGet(string city)
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;
            ChosenCity = city;

            var addresses = await _service.ReadAddressesAsync(true, false, city, 0, int.MaxValue);
            FriendsList = addresses.PageItems.SelectMany(a => a.Friends).ToList();

            return Page();
        }

        public ListOfFriendsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
