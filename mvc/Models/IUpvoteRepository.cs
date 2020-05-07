using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public interface IUpvoteRepository
    {
        IEnumerable<Upvote> GetAllUpvotes();
        Upvote GetUserUpvote(ApplicationUser user, Report report);

        int GetUpvoteCount(int reportId);

        void CreateUpvote(Upvote vote);
        void DeleteUpvote(Upvote vote);
    }
}
