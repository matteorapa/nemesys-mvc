using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public interface IApplicationUserRepository
    {
        IEnumerable<ApplicationUser> GetAllUsers();
    }
}
