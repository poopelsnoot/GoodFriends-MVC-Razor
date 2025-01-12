using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public string ErrorMessage { get; set; } = null;
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }

        public async Task<IActionResult> OnGet(Guid friendId)
        {
            try
            {
                var Friend = await _service.ReadFriendAsync(friendId, false);
                FriendToEdit = new FriendCUdto(Friend) {PetsId = Friend.Pets.Select(p => p.PetId).ToList(), QuotesId = Friend.Quotes.Select(q => q.QuoteId).ToList()};

                if(Friend.Address != null)
                {
                    AddressToEdit = new AddressCUdto(Friend.Address);
                }
                else
                {
                    AddressToEdit = new AddressCUdto();
                }

                UserHasAddress = !string.IsNullOrEmpty(Friend.Address?.StreetAddress);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSave(List<Guid> petsId, List<Guid> quotesId)
        {
            if (!IsValid())
            {
                return Page();
            }
    
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
            
            FriendToEdit.PetsId = petsId;
            FriendToEdit.QuotesId = quotesId;

            await _service.UpdateFriendAsync(FriendToEdit);
            
            return RedirectToPage("FriendDetails", new { friendId = FriendToEdit.FriendId });
        }

        public EditFriendModel(IFriendsService service)
        {
            _service = service;
        }

        private bool IsValid(string[] validateOnlyKeys = null)
        {
            InvalidKeys = ModelState.Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

            if (validateOnlyKeys != null)
            {
                InvalidKeys = InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
            }
            if (!UserHasAddress)
            {
                InvalidKeys = InvalidKeys.Where(s =>
                    !s.Key.StartsWith("AddressToEdit.", StringComparison.OrdinalIgnoreCase));
            }

            InvalidKeys = InvalidKeys.Where(s => 
                !s.Key.StartsWith("pets", StringComparison.OrdinalIgnoreCase) &&
                !s.Key.StartsWith("quotes", StringComparison.OrdinalIgnoreCase));
            
            ValidationErrorMsgs = InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            HasValidationErrors = InvalidKeys.Any();

            return !HasValidationErrors;
        }
    }
}