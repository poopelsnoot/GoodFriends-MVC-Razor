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
        public async Task<IActionResult> OnGet(string city, int page = 1)
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;
            ChosenCity = city;
            CurrentPage = page;

            var addresses = await _service.ReadAddressesAsync(true, false, city, 0, int.MaxValue);
            FriendsList = addresses.PageItems.SelectMany(a => a.Friends).Skip((page-1) * 10).Take(10).ToList();

            TotalPages = (int)Math.Ceiling((double)FriendsList.Count() / 10);

            return Page();
        }

        public ListOfFriendsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
