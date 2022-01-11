using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCard
{
    public class DataAccessLayer
    {
        public List<User> registerdUsers { get; set; }
        public Queue<Package> packages { get; set; }

        public DataAccessLayer()
        {
            registerdUsers = new List<User>();
            packages = new Queue<Package>();
        }

        #region Manage User Function

        public User? GetUser(UserLoginData loginData)
        {
            return registerdUsers.Find((user) => user.Name == loginData.UserName && user.Password == loginData.Password);
        }

        public User? GetUserByName(String username)
        {
            return registerdUsers.Find((user) => user.Name == username);
        }

        public bool AddUser(User newUser)
        {
            var existingUser = registerdUsers.Find(eUser => eUser.Name == newUser.Name);
            if (existingUser != null)
            {
                return false;
            }

            registerdUsers.Add(newUser);
            return true;
        }

        public bool UpdateUser(User user)
        {
            try
            {
                var index = registerdUsers.FindIndex(registeredUser => registeredUser.Name == user.Name);
                registerdUsers[index] = user;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public List<String> GetUserScores()
        {
            List<String> scores = new List<String>();
            foreach (var user in registerdUsers)
            {
                if(user.Name.ToUpper() != "ADMIN")
                {
                    scores.Add(user.Name + ": " + user.Score);
                }
            }

            return scores;
        }

        #endregion


        #region manage game functions
        public bool AddPackage(Package package)
        {
            packages.Enqueue(package);
            return true;
        }

        #endregion

    }
}
