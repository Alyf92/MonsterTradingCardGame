using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Damage { get; private set; }
        public ElementType ElementType { get; private set; }
        public CardType CardType { get; private set; }

        public Card(string id, string name, string damage)
        {
            Id = id;
            Name = name;
            String damageWithotDecimalPlaces;

            if(damage.Contains('.'))
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
