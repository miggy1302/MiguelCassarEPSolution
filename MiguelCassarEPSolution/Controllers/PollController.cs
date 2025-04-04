using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        //Method Incejtion here
        [HttpGet]
        public async Task<IActionResult> List([FromServices] PollRepository _myRepo)
        {
            var list = await _myRepo.GetPolls();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create([FromServices] PollRepository _myRepo)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromServices] PollRepository _myRepo, Poll p)
        {
            if (ModelState.IsValid)
            {
                await _myRepo.AddPoll(p);
                TempData["message"] = "Poll was added successfully";
                return RedirectToAction("List");
            }

            return View(p);

        }
    }
}
