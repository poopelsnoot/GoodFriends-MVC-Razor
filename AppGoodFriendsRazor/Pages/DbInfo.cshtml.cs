using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace MyApp.Namespace
{
    public class DbInfoModel : PageModel
    {
        readonly IFriendsService _service;
        public async Task<IActionResult> OnGet()
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;

            var nrFriendsSweden = dbInfo.Friends.Where(f => f.Country == "Sweden").Sum(f => f.NrFriends);
            return Page();
        }
        
        public DbInfoModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
