using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _appDbContext.Users;
        }
        public IEnumerable<User> GetAllInvestigators()
        {
            return _appDbContext.Users.Where(u => u.IsInvestigator == true);
        }
        public IEnumerable<User> GetAllReporters()
        {
            return _appDbContext.Users.Where(u => u.IsInvestigator == false);
        }

        public User GetUser(string userEmail)
        {
            return _appDbContext.Users.FirstOrDefault(u => u.UserEmail == userEmail);
        }
    }
}
