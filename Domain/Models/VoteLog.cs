using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class VoteLog
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // This links to AspNetUsers

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        [Required]
        public int PollId { get; set; }

        public DateTime VotedAt { get; set; }
    }
}
