using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace MyApp.Namespace
{
    public class DbInfoModel : PageModel
    {
        readonly IFriendsService _service;

        public int NrFriendsSweden { get; set; }
        public int NrCitiesSweden { get; set; }
        
        public async Task<IActionResult> OnGet()
        {
            GstUsrInfoAllDto dbInfo = await _service.InfoAsync;

            NrFriendsSweden = dbInfo.Friends.Where(f => f.Country == "Sweden").Sum(f => f.NrFriends);
            NrCitiesSweden = dbInfo.Friends.Count(f => f.Country == "Sweden");
            return Page();
        }
        
        public DbInfoModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
