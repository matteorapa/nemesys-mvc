using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            return _appDbContext.Upvotes.Include(u => u.User);
        }

        public Upvote GetUserUpvote(ApplicationUser user, Report rep)
        {
            return _appDbContext.Upvotes.FirstOrDefault(u => u.User == user && u.Report == rep);
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
