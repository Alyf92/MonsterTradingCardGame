namespace MonsterCard
{
    public class DataAccessLayerMock : IDataAccessLayer
    {
        public List<User> registerdUsers { get; set; }
        public Queue<Package> packages { get; set; }

        public DataAccessLayerMock()
        {
            registerdUsers = new List<User>();
            packages = new Queue<Package>();

            AddUser(new User(new UserLoginData { UserName = "Admin", Password = "Admin" }));
        }

        public bool AddPackage(Package package)
        {
            packages.Enqueue(package);
            return true;
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

        public bool BuyPackage(User user)
        {
            user.Cards.AddRange(packages.Dequeue().Cards.ToList());
            return true;
        }

        public bool ConfigureDeck(User user, Deck deck)
        {
            user.Decks.Add(deck);
            return true;
        }

        public List<Card> GetCards(User user)
        {
            return user.Cards;
        }

        public List<Deck> GetDecks(User user)
        {
            return user.Decks;
        }

        public User? GetUser(UserLoginData loginData)
        {
            return registerdUsers.Find((user) => user.Name == loginData.UserName && user.Password == loginData.Password);
        }

        public User? GetUserByName(string username)
        {
            return registerdUsers.Find((user) => user.Name == username);
        }

        public List<string> GetUserScores()
        {
            List<String> scores = new List<String>();
            foreach (var user in registerdUsers)
            {
                if (user.Name.ToUpper() != "ADMIN")
                {
                    scores.Add(user.Name + ": " + user.Score);
                }
            }

            return scores;
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
    }
}
