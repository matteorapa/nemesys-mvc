using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class UpvoteRepository : IUpvoteRepository
    {
        private readonly AppDbContext _appDbContext;

        public UpvoteRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Upvote> GetAllUpvotes()
        {
            return _appDbContext.Upvotes;
        }

        public Upvote GetUserUpvote(ApplicationUser user)
        {
            return _appDbContext.Upvotes.FirstOrDefault(u => u.User == user);
        }

        public int GetUpvoteCount(int reportId)
        {
            return _appDbContext.Upvotes.Count(u => u.Report.ReportId == reportId);
        }

        public void CreateUpvote(Upvote vote)
        {
            _appDbContext.Upvotes.Add(vote);
            _appDbContext.SaveChanges();
        }

        public void DeleteUpvote(Upvote vote)
        {
            _appDbContext.Upvotes.Remove(vote);
            _appDbContext.SaveChanges();
        }
    }
}
