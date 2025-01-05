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
            Friend = await _service.ReadFriendAsync(friendId, false);
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(Guid petId, Guid quoteId, Guid friendId)
        {
            if(petId != Guid.Parse("00000000-0000-0000-0000-000000000000")) { await _service.DeletePetAsync(petId); } 
            if(quoteId != Guid.Parse("00000000-0000-0000-0000-000000000000")) { await _service.DeleteQuoteAsync(quoteId); }

            return await OnGet(friendId);
        }
        
        public FriendDetailsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
