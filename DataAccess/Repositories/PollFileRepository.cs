using DataAccess.Repositories;
using Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class PollFileRepository : IPollRepository
{
    private readonly string _filePath = "polls.json";

    public async Task<List<Poll>> GetPolls()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Poll>();
        }

        using (var stream = File.OpenRead(_filePath))
        {
            return await JsonSerializer.DeserializeAsync<List<Poll>>(stream) ?? new List<Poll>();
        }
    }

    public async Task AddPoll(Poll newPoll)
    {
        var polls = await GetPolls(); // get existing ones
        newPoll.Id = polls.Count > 0 ? polls[^1].Id + 1 : 1; // auto-increment id
        newPoll.DateCreated = DateTime.Now;
        polls.Add(newPoll);

        var options = new JsonSerializerOptions { WriteIndented = true };
        using (var stream = File.Create(_filePath))
        {
            await JsonSerializer.SerializeAsync(stream, polls, options);
        }
    }

    public async Task<Poll?> GetPollByIdAsync(int id)
    {
        var polls = await GetPolls();
        return polls.FirstOrDefault(p => p.Id == id);
    }

    public async Task<bool> VoteAsync(int pollId, int option)
    {
        var polls = await GetPolls();
        var poll = polls.FirstOrDefault(p => p.Id == pollId);
        if (poll != null)
        {
            switch (option)
            {
                case 1: poll.Option1VotesCount++; break;
                case 2: poll.Option2VotesCount++; break;
                case 3: poll.Option3VotesCount++; break;
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, polls, options);
        }

        return true;
    }
}
