using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppGoodFriendsMVC.Models;
using Services;

namespace AppGoodFriendsMVC.Controllers;

public class FriendController : Controller
{
    private readonly ILogger<FriendController> _logger;
    IQuoteService _service = null;
    readonly LatinService _latinService;

    public FriendController(ILogger<FriendController> logger, IQuoteService service, LatinService latinService)
    {
        _service = service;
        _logger = logger;
        _latinService = latinService;
    }

    //Will execute on a Get request
    [HttpGet]
    public IActionResult ModelList()
    {
        var vw = new ModelListViewModel();

        //Use the Service
        vw.Quotes = _service.ReadQuotes();
        //return View(vw);
        return View("ModelList", vw);
    }

    //Will execute on a Get request
    [HttpGet]
    [Route("/Friend/Search")]
    [Route("quotes/{search?}")]
    public IActionResult Search(string search, int pagenr)
    {
        var vwm = new SearchViewModel(){ThisPageNr = pagenr, SearchFilter = search};
        
        //Pagination
        vwm.UpdatePagination(_service);

        //Use the Service
        vwm.Quotes = _service.ReadQuotes(vwm.ThisPageNr, vwm.PageSize, vwm.SearchFilter);

        return View(vwm);
    }

    [HttpPost]
    public IActionResult Find(SearchViewModel vwm)
    {
        //Pagination
        vwm.UpdatePagination(_service);

        //Use the Service
        vwm.Quotes = _service.ReadQuotes(vwm.ThisPageNr, vwm.PageSize, vwm.SearchFilter);

        return View("Search", vwm);
    }
    public IActionResult LatinList(string pagenr, string search)
    {
        var vm = new LatinViewModel();

        //Read a QueryParameters
        if (int.TryParse(pagenr, out int _pagenr))
        {
            vm.ThisPageNr = _pagenr;
        }

        vm.SearchFilter = search; //Request.Query["search"];

        //Pagination
        vm.UpdatePagination(_latinService);

        //Use the Service
        vm.Latins = _latinService.ReadSentences(vm.ThisPageNr, vm.PageSize, vm.SearchFilter);

        return View(vm);
    }

    public IActionResult LatinSearch(LatinViewModel vm)
    {
        //Pagination
         vm.UpdatePagination(_latinService);

        //Use the Service
        vm.Latins = _latinService.ReadSentences(vm.ThisPageNr, vm.PageSize, vm.SearchFilter);

        //Page is rendered as the postback is part of the form tag
        return View("LatinList", vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

