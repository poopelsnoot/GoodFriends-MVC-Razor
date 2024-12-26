using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Models;
using Services;

namespace MyApp.Namespace
{
    public class FriendDetailsModel : PageModel
    {
        readonly IFriendsService _service;
        public IFriend Friend { get; set; }
        public async Task<IActionResult> OnGet(Guid friendId)
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;

            Friend = await _service.ReadFriendAsync(friendId, false);

            return Page();
        }
        
        public FriendDetailsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
