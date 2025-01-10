using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models.DTO;

namespace AppGoodFriendsMVC.Models;

public class EditFriendViewModel
{
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
}