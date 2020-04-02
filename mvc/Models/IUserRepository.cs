using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetAllInvestigators();
        IEnumerable<User> GetAllReporters();

        User GetUser(string userEmail);

        //User RegisterUser();
        //User EditUser(string userEmail);
        //User DeleteUser(string userEmail);

    }
}
