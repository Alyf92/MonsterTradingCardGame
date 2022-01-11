using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCard
{
    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck(Card card1, Card card2, Card card3, Card card4)
        {
            Cards = new List<Card>();
            Cards.Add(card1);
            Cards.Add(card2);
            Cards.Add(card3);
            Cards.Add(card4);
        }
    }
}
