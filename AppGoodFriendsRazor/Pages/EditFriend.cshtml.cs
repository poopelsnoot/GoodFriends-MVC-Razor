using DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace MyApp.Namespace
{
    public class EditFriendModel : PageModel
    {
        readonly IFriendsService _service;
        
        [BindProperty]
        public FriendCUdto FriendToEdit { get; set; }
        // [BindProperty]
        // public AddressCUdto AddressToEdit { get; set; }

        public async Task<IActionResult> OnGet(Guid friendId)
        {
            var Friend = await _service.ReadFriendAsync(friendId, false);

            FriendToEdit = new FriendCUdto(Friend);
            //AddressToEdit = new AddressCUdto(Friend.Address);

            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            var testFriend = await _service.UpdateFriendAsync(FriendToEdit);
            //await _service.UpdateAddressAsync(AddressToEdit);
            
            
            return Page();
        }

        public EditFriendModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
