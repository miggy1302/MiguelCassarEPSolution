using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilter;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;

        public PollController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        //Method Incejtion here
        [HttpGet]
        public async Task<IActionResult> List([FromServices] IPollRepository _myRepo)
        {
            var polls = await _myRepo.GetPolls();
            return View(polls);
        }

        [Authorize]
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

        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(OneVotePerUserFilter))]
        public async Task<IActionResult> Vote(int pollId, int selectedOption, [FromServices] IPollRepository _myRepo, [FromServices] VoteLogRepository _voteRepo, VoteLog v)
        {
            var userId = _userManager.GetUserId(User);
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

            await _voteRepo.LogVoteAsync(userId, pollId);

            TempData["message"] = "Vote submitted successfully!";
            return RedirectToAction("List");
        }


        [Authorize]
        [HttpGet]
        public IActionResult Create([FromServices] IPollRepository _myRepo)
        {
            return View();
        }

        [Authorize]
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
