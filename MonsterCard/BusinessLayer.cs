using System.Security.Cryptography;
using System.Text;

namespace MonsterCard
{
    public class BusinessLayer
    {
        private IDataAccessLayer _dataAccessLayer;
        private const string _algorithmName = "HmacSHA256";
        private const string _key = "rz8LuOtFBXphj9WQfvFh";
        private Stack<Player> _players = new Stack<Player>();

        public BusinessLayer()
        {
            _dataAccessLayer = new DataAccessLayer();
        }

        public BusinessLayer(IDataAccessLayer dl)
        {
            _dataAccessLayer = dl;
        }

        #region Registration and Login Functions

        public bool RegisterUser(UserLoginData loginData)
        {
            if (loginData.UserName == null
                || loginData.UserName.Length == 0
                || loginData.Password == null
                || loginData.Password.Length == 0)
            {
                throw new ArgumentNullException(nameof(loginData));
            }

            return _dataAccessLayer.AddUser(new User(loginData));
        }

        public string Login(UserLoginData loginData)
        {
            if (loginData.UserName == null
                || loginData.UserName.Length == 0
                || loginData.Password == null
                || loginData.Password.Length == 0)
            {
                throw new ArgumentNullException(nameof(loginData));
            }

            var user = _dataAccessLayer.GetUser(loginData);


            if (user == null) { return null; }

            return GenerateToken(user.Name, user.Password);

        }

        public bool UpdateUserData(UserData data, string userName, string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var user = GetUserFromToken(token);

            if (user == null || user.Name != userName)
            {
                throw new UnauthorizedAccessException();
            }

            user.FirstName = data.FirstName;
            user.LastName = data.LastName;
            user.Bio = data.Bio;

            return _dataAccessLayer.UpdateUser(user);
        }

        public UserData GetUserData(string userName, string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var user = GetUserFromToken(token);

            if (user == null || user.Name != userName)
            {
                throw new UnauthorizedAccessException();
            }

            return new UserData(user.FirstName, user.LastName, user.Bio, user.Name);
        }

        #endregion

        #region Security Token Functions

        public string GenerateToken(string username, string password)
        {
            string hash = string.Join(":", new string[] { username });
            string hashLeft = "";
            string hashRight = "";

            using (var hmac = HMAC.Create(_algorithmName))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(password));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));

                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join(":", new string[] { username });
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
        }

        public string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, _key });

            using (var hmac = HMAC.Create(_algorithmName))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(_key);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));

                return Convert.ToBase64String(hmac.Hash);
            }
        }

        public User? GetUserFromToken(string token)
        {
            bool isTokenValid = false;
            string key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            User? user = null;

            // Split the parts.
            string[] parts = key.Split(new char[] { ':' });
            if (parts.Length == 2)
            {
                string username = parts[1];
                user = _dataAccessLayer.GetUserByName(username);

                if (user != null)
                {
                    string computedToken = GenerateToken(username, user.Password);

                    // Compare the computed token with the one supplied and ensure they match.
                    isTokenValid = (token == computedToken);
                }
            }

            return isTokenValid ? user : null;
        }

        #endregion

        #region manage packages and cards functions

        public bool AddPackage(Package package, string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var user = GetUserFromToken(token);

            if (user == null || user.Name != "Admin")
            {
                throw new UnauthorizedAccessException();
            }

            return _dataAccessLayer.AddPackage(package);
        }

        public bool BuyPackage(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var user = GetUserFromToken(token);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (user.Coins < 5)
            {
                return false;
            }

            return _dataAccessLayer.BuyPackage(user);
            /* var package = _dataAccessLayer.packages.Dequeue();

             foreach(var card in package.Cards)
             {
                 user.Cards.Add(card);
             }

             user.Coins -= 5;

             _dataAccessLayer.UpdateUser(user);
             return true;*/
        }

        public List<Card> GetCards(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var user = GetUserFromToken(token);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            return _dataAccessLayer.GetCards(user);
        }

        #endregion

        #region manage decks
        public bool ConfigureDeck(String[] ids, String token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("Token");
            }

            var user = GetUserFromToken(token);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (ids.Length != 4)
            {
                throw new ArgumentException("Number of Cards");
            }

            Card[] cards = new Card[4];

            var userCards = _dataAccessLayer.GetCards(user);

            for (var i = 0; i < ids.Length; i++)
            {
                var card = userCards.Find(card => card.Id == ids[i]);

                if (card == null)
                {
                    throw new Exception("Card not found");
                }

                cards[i] = card;
            }

            return _dataAccessLayer.ConfigureDeck(user, new Deck(cards[0], cards[1], cards[2], cards[3]));
        }

        public List<Deck> GetDecks(String token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("Token");
            }

            var user = GetUserFromToken(token);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            return _dataAccessLayer.GetDecks(user);
        }
        #endregion

        #region Stats & Score

        public int GetScore(String token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("Token");
            }

            var user = GetUserFromToken(token);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            return user.Score;
        }

        public List<String> GetScoreBoard(String token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("Token");
            }

            var user = GetUserFromToken(token);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            return _dataAccessLayer.GetUserScores();
        }

        public List<String> AddUserToBattle(String token, int deckIndex)
        {
            List<String> result = new List<String>();

            if (token == null)
            {
                throw new ArgumentNullException("Token");
            }

            var user = GetUserFromToken(token);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var decks = _dataAccessLayer.GetDecks(user);
            var deck = decks[deckIndex];

            if (deck == null)
            {
                throw new Exception("Deck not found");
            }

            user.Decks = decks;

            _players.Push(new Player(user, deckIndex));

            if (_players.Count >= 2)
            {
                var player1 = _players.Pop();
                var player2 = _players.Pop();

                result = StartBattle(player1, player2);
            }
            else
            {
                result.Add("Waiting for other players to join...");
            }

            return result;
        }

        private List<String> StartBattle(Player player1, Player player2)
        {
            List<String> result = new List<string>();
            int rounds = 0;

            result.Add("Player 1: " + player1.User.Name + " VS Player 2: " + player2.User.Name);

            while (player1.User.Decks[player1.DeckIndex].Cards.Count > 0 && player2.User.Decks[player2.DeckIndex].Cards.Count > 0 && rounds < 100)
            {
                var roundResult = PlayRound(player1, player2);
                result.AddRange(roundResult);
                rounds++;
            }

            var user1 = player1.User;
            var user2 = player2.User;

            if (rounds == 100)
            {
                result.Add("Draw! No Winner");
            }
            else if (player1.User.Decks[player1.DeckIndex].Cards.Count == 0)
            {
                user1.Score = user1.Score - 5;
                user2.Score = user2.Score + 3;
                result.Add("Winner: " + player2.User.Name + "!");
            }
            else if (player2.User.Decks[player2.DeckIndex].Cards.Count == 0)
            {
                user2.Score = user2.Score - 5;
                user1.Score = user1.Score + 3;
                result.Add("Winner: " + player1.User.Name + "!");
            }

            _dataAccessLayer.UpdateUser(user1);
            _dataAccessLayer.UpdateUser(user2);
            return result;
        }

        private List<string> PlayRound(Player player1, Player player2)
        {
            var random = new Random();
            int player1CardIndex = random.Next(player1.User.Decks[player1.DeckIndex].Cards.Count);
            int player2CardIndex = random.Next(player2.User.Decks[player2.DeckIndex].Cards.Count);
            var card1 = player1.User.Decks[player1.DeckIndex].Cards[player1CardIndex];
            var card2 = player2.User.Decks[player2.DeckIndex].Cards[player2CardIndex];

            List<String> result = new List<string>();

            result.Add("Start Round..");

            result.Add("Player 1: " + card1.Name + " VS Player 2: " + card2.Name);

            if (card1.CardType == CardType.Monster && card2.CardType == CardType.Monster)
            {
                if (card1.Damage > card2.Damage)
                {
                    result.Add(card1.Name + " defeats " + card2.Name);

                    MoveCardToWinner(player1, player2, card2, 1);
                }
                else if (card2.Damage > card1.Damage)
                {
                    result.Add(card2.Name + " defeats " + card1.Name);

                    MoveCardToWinner(player1, player2, card1, 2);
                }
                else
                {
                    result.Add("Draw");
                }
            }
            else
            {
                var totalDamage1 = card1.Damage;
                var totalDamage2 = card2.Damage;

                var effectiveCard = getMoreEffectiveCard(card1, card2);

                if (effectiveCard == 1)
                {
                    totalDamage1 *= 2;
                    totalDamage2 /= 2;
                }
                else if (effectiveCard == 2)
                {
                    totalDamage2 *= 2;
                    totalDamage1 /= 2;
                }

                if (totalDamage1 > totalDamage2)
                {
                    result.Add(card1.Name + " defeats " + card2.Name);

                    MoveCardToWinner(player1, player2, card2, 1);
                }
                else if (totalDamage2 > totalDamage1)
                {
                    result.Add(card2.Name + " defeats " + card1.Name);

                    MoveCardToWinner(player1, player2, card1, 2);
                }
                else
                {
                    result.Add("Draw");
                }
            }

            result.Add("Finish Round..");

            return result;
        }

        private void MoveCardToWinner(Player player1, Player player2, Card card, int winner)
        {
            if (winner == 1)
            {
                var user2 = player2.User;
                //int indexToRemove = user2.Cards.FindIndex(c => c.Id == card.Id);
                //user2.Cards.RemoveAt(indexToRemove);
                user2.Decks[player2.DeckIndex].Cards.Remove(card);
                // _dataAccessLayer.UpdateUser(user2);

                var user1 = player1.User;
                //user1.Cards.Add(card);
                user1.Decks[player1.DeckIndex].Cards.Add(card);
                //_dataAccessLayer.UpdateUser(user2);
            }
            else if (winner == 2)
            {
                var user1 = player1.User;
                //int indexToRemove = user1.Cards.FindIndex(c => c.Id == card.Id);
                //user1.Cards.RemoveAt(indexToRemove);
                user1.Decks[player1.DeckIndex].Cards.Remove(card);
                //_dataAccessLayer.UpdateUser(user1);

                var user2 = player2.User;
                //user2.Cards.Add(card);
                user2.Decks[player2.DeckIndex].Cards.Add(card);
                //_dataAccessLayer.UpdateUser(user2);
            }
        }

        private int getMoreEffectiveCard(Card card1, Card card2)
        {
            if (card1.ElementType == ElementType.Water)
            {
                if (card2.ElementType == ElementType.Water) return 0;
                if (card2.ElementType == ElementType.Fire) return 1;
                if (card2.ElementType == ElementType.Normal) return 2;
            }
            else if (card1.ElementType == ElementType.Fire)
            {
                if (card2.ElementType == ElementType.Fire) return 0;
                if (card2.ElementType == ElementType.Water) return 2;
                if (card2.ElementType == ElementType.Normal) return 1;
            }
            else
            {
                if (card2.ElementType == ElementType.Normal) return 0;
                if (card2.ElementType == ElementType.Water) return 1;
                if (card2.ElementType == ElementType.Fire) return 2;
            }

            return 0;
        }

        #endregion
    }
}
