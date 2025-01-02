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

        public async Task<IActionResult> OnPostDelete(Guid petId, Guid friendId)
        {
            await _service.DeletePetAsync(petId);

            return await OnGet(friendId);
        }
        
        public FriendDetailsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
