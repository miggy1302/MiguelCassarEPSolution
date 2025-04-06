using DataAccess.DataContext;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class VoteLogRepository
    {
        private PollDbContext _myContext;

        public VoteLogRepository(PollDbContext myContext)
        {
            _myContext = myContext;
        }

        public async Task<bool> HasUserVotedAsync(string userId, int pollId)
        {
            return await _myContext.VoteLogs.AnyAsync(v => v.UserId == userId && v.PollId == pollId);
        }

        public async Task LogVoteAsync(string userId, int pollId)
        {
            var vote = new VoteLog { UserId = userId, PollId = pollId};
            _myContext.VoteLogs.Add(vote);
            await _myContext.SaveChangesAsync();
        }
    }
}
