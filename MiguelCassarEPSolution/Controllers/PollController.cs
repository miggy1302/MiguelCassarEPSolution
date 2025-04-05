using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        //Method Incejtion here
        [HttpGet]
        public async Task<IActionResult> List([FromServices] IPollRepository _myRepo)
        {
            var polls = await _myRepo.GetPolls();
            return View(polls);
        }

        [HttpGet]
        public async Task<IActionResult> Vote([FromServices] IPollRepository _myRepo, int id)
        {
            var poll = await _myRepo.GetPollByIdAsync(id);
            if (poll == null)
            {
                TempData["error"] = "Poll not found.";
                return RedirectToAction("List");
            }

            return View(poll);
        }

        [HttpPost]
        public async Task<IActionResult> Vote(int pollId, int selectedOption, [FromServices] IPollRepository _myRepo)
        {
            if (selectedOption < 1 || selectedOption > 3)
            {
                TempData["error"] = "Invalid option selected.";
                return RedirectToAction("Vote", new { id = pollId });
            }

            var success = await _myRepo.VoteAsync(pollId, selectedOption);
            if (!success)
            {
                TempData["error"] = "Unable to cast vote. Poll not found or invalid input.";
                return RedirectToAction("List");
            }

            TempData["message"] = "Vote submitted successfully!";
            return RedirectToAction("List");
        }



        [HttpGet]
        public IActionResult Create([FromServices] IPollRepository _myRepo)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromServices] IPollRepository _myRepo, Poll p)
        {
            if (ModelState.IsValid)
            {
                await _myRepo.AddPoll(p);
                TempData["message"] = "Poll was added successfully";
                return RedirectToAction("List");
            }

            return View(p);

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> View([FromServices] IPollRepository _myRepo, int id)
        {
            var poll = await _myRepo.GetPollByIdAsync(id);
            if (poll == null)
            {
                TempData["error"] = "Poll not found.";
                return RedirectToAction("List");
            }

            return View(poll);
        }
    }
}
