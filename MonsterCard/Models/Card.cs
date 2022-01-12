namespace MonsterCard
{
    public enum CardType
    {
        Spell,
        Monster
    }

    public enum ElementType
    {
        Water,
        Fire,
        Normal
    }

    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
        public ElementType ElementType { get; set; }
        public CardType CardType { get; set; }


        public Card(string id, string name, string damage)
        {
            Id = id;
            Name = name;
            String damageWithotDecimalPlaces;

            if (damage.Contains('.'))
            {
                damageWithotDecimalPlaces = damage.Substring(0, damage.IndexOf('.'));
            }
            else
            {
                damageWithotDecimalPlaces = damage;
            }

            Damage = Int32.Parse(damageWithotDecimalPlaces);

            if (name.Contains("Spell"))
            {
                CardType = CardType.Spell;
            }
            else
            {
                CardType = CardType.Monster;
            }

            if (name.Contains("Fire"))
            {
                ElementType = ElementType.Fire;
            }
            else if (name.Contains("Water"))
            {
                ElementType = ElementType.Water;
            }
            else
            {
                ElementType = ElementType.Normal;
            }
        }
    }
}
