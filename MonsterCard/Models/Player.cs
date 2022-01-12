namespace MonsterCard
{
    public class Player
    {
        public User User { get; set; }
        public int DeckIndex { get; set; }

        public Player(User user, int index)
        {
            this.User = user;
            this.DeckIndex = index;
        }
    }
}
