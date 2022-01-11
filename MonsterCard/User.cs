using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCard
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public int Coins { get; set; }
        public List<Card> Cards { get; set; }
        public List<Deck> Decks { get; set; }
        public int Score { get; set; }

        public User(UserLoginData data)
        {
            Name = data.UserName;
            Password = data.Password;
            Cards = new List<Card>();
            Decks = new List<Deck>();
            Coins = 20;
            Score = 100;
        }
    }
}
