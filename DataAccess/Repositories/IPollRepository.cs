using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IPollRepository
    {
        Task<List<Poll>> GetPolls();
        Task AddPoll(Poll poll);
        Task<Poll?> GetPollByIdAsync(int id);
        Task<bool> VoteAsync(int pollId, int option);
    }
}
