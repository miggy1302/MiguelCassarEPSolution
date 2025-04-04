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
    public class PollRepository
    {
        private PollDbContext myContext;

        //Constructor Injection - Reference for me 
        public PollRepository(PollDbContext _myContext)
        {
            myContext = _myContext;
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



        public async Task CreatePoll(string title, string option1Text, string option2Text, string option3Text)
        {
            var poll = new Poll
            {
                Title = title,
                Option1Text = option1Text,
                Option2Text = option2Text,
                Option3Text = option3Text,
                Option1VotesCount = 0,
                Option2VotesCount = 0,
                Option3VotesCount = 0,
                DateCreated = DateTime.UtcNow
            };

            myContext.Polls.Add(poll);
            await myContext.SaveChangesAsync();
        }

        public async Task<List<Poll>> GetPolls()
        {
            return await myContext.Polls
                .AsNoTracking() // optimization: no tracking needed for read-only data
                .ToListAsync();
        }
    }
}
