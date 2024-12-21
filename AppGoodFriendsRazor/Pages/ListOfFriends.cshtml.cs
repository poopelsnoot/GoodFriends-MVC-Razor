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
        private List<IFriend> AllFriends { get; set; } = new List<IFriend>();
        public List<IFriend> FriendsList { get; set; } = new List<IFriend>();
        public string ChosenCity { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; } 

        [BindProperty]
        public string SearchFilter { get; set; }
        public async Task<IActionResult> OnGet(string city, int pageNumber = 1)
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;
            ChosenCity = city;
            CurrentPage = pageNumber;
            SearchFilter = Request.Query["search"];
            if (SearchFilter == null) { SearchFilter = ""; }

            var Addresses = await _service.ReadAddressesAsync(true, false, city, 0, int.MaxValue);
            AllFriends = Addresses.PageItems.SelectMany(a => a.Friends).ToList();

            TotalPages = (int)Math.Ceiling((double)AllFriends.Count() / 10);
            FriendsList = AllFriends.Where(f => f.FirstName.Contains(SearchFilter) || f.LastName.Contains(SearchFilter)).Skip((CurrentPage-1) * 10).Take(10).ToList();

            return Page();
        }

        public IActionResult OnPostSearch()
        {
            FriendsList = AllFriends.Where(f => f.FirstName.Contains(SearchFilter) || f.LastName.Contains(SearchFilter)).Take(10).ToList();

            return Page();
        }

        public ListOfFriendsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
