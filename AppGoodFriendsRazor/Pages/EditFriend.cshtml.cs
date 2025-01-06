using System.Diagnostics.Eventing.Reader;
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
        [BindProperty]
        public AddressCUdto AddressToEdit { get; set; }
        [BindProperty]
        public bool UserHasAddress { get; set; }

        public async Task<IActionResult> OnGet(Guid friendId)
        {
            var Friend = await _service.ReadFriendAsync(friendId, false);
            FriendToEdit = new FriendCUdto(Friend);

            if(Friend.Address != null)
            {
                AddressToEdit = new AddressCUdto(Friend.Address);
            }
            else
            {
                AddressToEdit = new AddressCUdto();
            }

            UserHasAddress = !string.IsNullOrEmpty(Friend.Address?.StreetAddress);

            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {

            if (AddressToEdit.AddressId == null && UserHasAddress)
            {
                var newAddress = await _service.CreateAddressAsync(AddressToEdit);
                AddressToEdit = new AddressCUdto(newAddress);
            }
            else if (AddressToEdit.AddressId != null && UserHasAddress)
            {
                await _service.UpdateAddressAsync(AddressToEdit);
            }

            if(UserHasAddress) { FriendToEdit.AddressId = AddressToEdit.AddressId; }
            await _service.UpdateFriendAsync(FriendToEdit);
            
            return Page();
        }

        public EditFriendModel(IFriendsService service)
        {
            _service = service;
        }
    }

}
