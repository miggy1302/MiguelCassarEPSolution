using DataAccess.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Presentation.ActionFilter
{
    public class OneVotePerUserFilter : IAsyncActionFilter
    {
        private readonly PollDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OneVotePerUserFilter(PollDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userId = _userManager.GetUserId(user); // Get the logged-in user's ID

            if (context.ActionArguments.TryGetValue("pollId", out var pollIdObj) && pollIdObj is int pollId)
            {
                var alreadyVoted = await _context.VoteLogs
                    .AnyAsync(v => v.PollId == pollId && v.UserId == userId);

                if (alreadyVoted)
                {
                    var controller = (Controller)context.Controller;
                    controller.TempData["Error"] = "You have already voted in this poll.";

                    context.Result = new RedirectToActionResult("List", "Poll", null);
                    return;
                }
            }

            await next();
        }
    }
}
