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
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public async Task<IActionResult> OnGet(string city, int pageNumber = 1)
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;
            ChosenCity = city;
            CurrentPage = pageNumber;

            var addresses = await _service.ReadAddressesAsync(true, false, city, 0, int.MaxValue);
            var AllFriends = addresses.PageItems.SelectMany(a => a.Friends).ToList();

            TotalPages = (int)Math.Ceiling((double)AllFriends.Count() / 10);
            FriendsList = AllFriends.Skip((pageNumber-1) * 10).Take(10).ToList();

            return Page();
        }

        public ListOfFriendsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
