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
        private List<IFriend> AllFriendsInCity { get; set; } = new List<IFriend>();
        public List<IFriend> FriendsList { get; set; } = new List<IFriend>();
        public string ChosenCity { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; } 

        public async Task<IActionResult> OnGet(string city, int pageNumber = 1)
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;
            ChosenCity = city;
            CurrentPage = pageNumber;

            if (ChosenCity != "Unknown")
            {
                var AddressesInCity = await _service.ReadAddressesAsync(true, false, ChosenCity, 0, int.MaxValue);
                var unseededAddressesInCity = await _service.ReadAddressesAsync(false, false, ChosenCity, 0, int.MaxValue);
                AddressesInCity.PageItems.AddRange(unseededAddressesInCity.PageItems);

                AllFriendsInCity = AddressesInCity.PageItems.SelectMany(a => a.Friends).ToList();
            }
            else
            {
                var allFriends = await _service.ReadFriendsAsync(true, false, "", 0, int.MaxValue);
                var unseededFriends = await _service.ReadFriendsAsync(false, false, "", 0, int.MaxValue);
                allFriends.PageItems.AddRange(unseededFriends.PageItems);
                
                AllFriendsInCity = allFriends.PageItems.Where(f => f.Address == null).ToList();
            }

            TotalPages = (int)Math.Ceiling((double)AllFriendsInCity.Count() / 10);
            FriendsList = AllFriendsInCity.Skip((CurrentPage-1) * 10).Take(10).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostDelete(Guid id, string city, int pageNumber)
        {
            await _service.DeleteFriendAsync(id);

            return await OnGet(city, pageNumber);
        }

        public ListOfFriendsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
