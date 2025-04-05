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
    public class PollRepository : IPollRepository
    {
        private PollDbContext myContext;

        //Constructor Injection - Reference for me 
        public PollRepository(PollDbContext _myContext)
        {
            myContext = _myContext;
        }

        public async Task<List<Poll>> GetPolls()
        {
            return await myContext.Polls
                .AsNoTracking()
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();
        }

        public async Task AddPoll(Poll p)
        {
            p.DateCreated = DateTime.Now;
            p.Option1VotesCount = 0;
            p.Option2VotesCount = 0;
            p.Option3VotesCount = 0;

            myContext.Polls.Add(p);
            await myContext.SaveChangesAsync();
        }

        public async Task<Poll?> GetPollByIdAsync(int id)
        {
            return await myContext.Polls
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> VoteAsync(int pollId, int optionNumber)
        {
            var poll = await myContext.Polls.FindAsync(pollId);
            if (poll == null) return false;

            switch (optionNumber)
            {
                case 1:
                    poll.Option1VotesCount++;
                    break;
                case 2:
                    poll.Option2VotesCount++;
                    break;
                case 3:
                    poll.Option3VotesCount++;
                    break;
                default:
                    return false;
            }

            await myContext.SaveChangesAsync();
            return true;
        }

    }
}
