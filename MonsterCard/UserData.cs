using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCard
{
    public class UserData
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }

        public UserData(string firstName, string lastName, string bio, string userName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Bio = bio;
            this.UserName = userName;
        }

    }
}
