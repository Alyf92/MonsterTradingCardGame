namespace MonsterCard
{
    public class Package
    {
        public Card[] Cards { get; private set; }

        public Package(Card[] cards)
        {
            if (cards.Length != 5) {
                throw new ArgumentException("Cards Length Error");
            }

            Cards = cards;
        }
    }
}
