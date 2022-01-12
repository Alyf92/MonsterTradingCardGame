namespace MonsterCard
{
    public interface IDataAccessLayer
    {
        public User? GetUser(UserLoginData loginData);
        public User? GetUserByName(string username);
        public bool AddUser(User newUser);
        public bool UpdateUser(User user);
        public List<string> GetUserScores();
        public bool AddPackage(Package package);
        public bool BuyPackage(User user);
        public List<Card> GetCards(User user);
        public bool ConfigureDeck(User user, Deck deck);
        public List<Deck> GetDecks(User user);
    }

}
