using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public ApplicationUserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return _appDbContext.Users.Include(u => u.Reports).ToList();
        }
    }
}
