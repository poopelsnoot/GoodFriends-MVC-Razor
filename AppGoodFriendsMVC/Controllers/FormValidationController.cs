using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppStudiesMVC.Models;
using Services;
using static AppStudiesMVC.Models.FullValidationViewModel;
using AppStudies.SeidoHelpers;
using System.ComponentModel.DataAnnotations;
using Models;

namespace AppStudiesMVC.Controllers;

public class FormValidationController : Controller
{
    readonly ILogger<FormValidationController> _logger;
    readonly IQuoteService _service = null;

    public FormValidationController(ILogger<FormValidationController> logger, IQuoteService service)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult FullValidationListAdd()
    {
        var vwm = new FullValidationViewModel();
        vwm.QuotesIM = _service.ReadQuotes().Select(q => new FamousQuoteIM(q)).ToList();
        return View(vwm);
    }

    [HttpPost]
    public IActionResult FullValidationDelete(Guid quoteId, FullValidationViewModel vwm)
    {
        //Set the Quote as deleted, it will not be rendered
        vwm.QuotesIM.First(q => q.QuoteId == quoteId).StatusIM = StatusIM.Deleted;
        return View("FullValidationListAdd", vwm);
    }

    [HttpPost]
    public IActionResult FullValidationEdit(Guid quoteId, FullValidationViewModel vwm)
    {
        int idx = vwm.QuotesIM.FindIndex(q => q.QuoteId == quoteId);
        string[] keys = { $"QuotesIM[{idx}].editQuote",
                            $"QuotesIM[{idx}].editAuthor"};
        if (!ModelState.IsValidPartially(out ModelValidationResult validationResult, keys))
        {
            vwm.ValidationResult = validationResult;
            return View("FullValidationListAdd", vwm);
        }

        //Set the Quote as Modified, it will later be updated in the database
        var q = vwm.QuotesIM.First(q => q.QuoteId == quoteId);
        q.StatusIM = StatusIM.Modified;

        //Implement the changes
        q.Author = q.editAuthor;
        q.Quote = q.editQuote;
        return View("FullValidationListAdd", vwm);
    }

    [HttpPost]
    public IActionResult FullValidationAdd(FullValidationViewModel vwm)
    {
        string[] keys = { $"NewQuoteIM.Quote",
                            $"NewQuoteIM.Author"};
        if (!ModelState.IsValidPartially(out ModelValidationResult validationResult, keys))
        {
            vwm.ValidationResult = validationResult;
            return View("FullValidationListAdd", vwm);
        }

        //Set the Artist as Inserted, it will later be inserted in the database
        vwm.NewQuoteIM.StatusIM = StatusIM.Inserted;

        //Need to add a temp Guid so it can be deleted and editited in the form
        //A correct Guid will be created by the DTO when Inserted into the database
        vwm.NewQuoteIM.QuoteId = Guid.NewGuid();

        //Add it to the Input Models artists
        vwm.QuotesIM.Add(new FamousQuoteIM(vwm.NewQuoteIM));

        //Clear the NewArtist so another album can be added
        vwm.NewQuoteIM = new FamousQuoteIM();

        return View("FullValidationListAdd", vwm);
    }

    public IActionResult FullValidationUndo(FullValidationViewModel vwm)
    {
        //Reload the InputModel
        vwm.QuotesIM = _service.ReadQuotes().Select(q => new FamousQuoteIM(q)).ToList();
        return View("FullValidationListAdd", vwm);
    }

    public IActionResult FullValidationSave(FullValidationViewModel vwm)
    {
        //Note: Here I will not do any validation as all validation is done during the
        //OnPostEdit and OnPostAdd

        //Check if there are deleted quotes, if so simply remove them
        var _deletes = vwm.QuotesIM.FindAll(q => (q.StatusIM == StatusIM.Deleted));
        foreach (var item in _deletes)
        {
            //Remove from the database
            _service.DeleteQuote(item.QuoteId);
        }

        #region Add quotes
        //Check if there are any new quotes added, if so create them in the database
        var _newies = vwm.QuotesIM.FindAll(q => (q.StatusIM == StatusIM.Inserted));
        foreach (var item in _newies)
        {
            //Create the corresposning model
            var model = item.UpdateModel(new FamousQuote());

            //create in the database
            _service.CreateQuote(model);
        }
        #endregion

        //Check if there are any modified quotes , if so update them in the database
        var _modyfies = vwm.QuotesIM.FindAll(a => (a.StatusIM == StatusIM.Modified));
        foreach (var item in _modyfies)
        {
            //get model
            var model = _service.ReadQuote(item.QuoteId);

            //update the changes and save
            model = item.UpdateModel(model);
            _service.UpdateQuote(model);
        }

        //Reload the InputModel
        vwm.QuotesIM = _service.ReadQuotes().Select(q => new FamousQuoteIM(q)).ToList();
        return View("FullValidationListAdd", vwm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

