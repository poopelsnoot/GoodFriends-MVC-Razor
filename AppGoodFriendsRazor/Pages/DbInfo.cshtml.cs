using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace MyApp.Namespace
{
    public class DbInfoModel : PageModel
    {
        readonly IFriendsService _service;
        public async Task<IActionResult> OnGet()
        {
            var dbInfo = await _service.InfoAsync;
            return Page();
        }
        
        public DbInfoModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
