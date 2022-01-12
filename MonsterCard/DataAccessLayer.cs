using Npgsql;

namespace MonsterCard
{
    public class DataAccessLayer : IDataAccessLayer
    {

        private static string connectionString = "Host=localhost;Username=postgres;Database=game";
        private NpgsqlConnection _connection = new NpgsqlConnection(connectionString);

        public DataAccessLayer()
        {
            _connection.Open();

            //Create Admin User if it doesn't exist
            AddUser(new User(new UserLoginData { UserName = "Admin", Password = "Admin" }));
        }

        #region Manage User Function

        public User? GetUser(UserLoginData loginData)
        {
            User user = null;

            var cmd = new NpgsqlCommand("SELECT * FROM game_user WHERE name=$1 AND password=$2", _connection)
            {
                Parameters =
                    {
                        new() { Value = loginData.UserName },
                        new() { Value = loginData.Password }
                    }
            };

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    user = new User(new UserLoginData { UserName = reader.GetString(0), Password = reader.GetString(1) });
                    user.Coins = reader.GetInt32(2);
                    user.Score = reader.GetInt32(3);
                    user.FirstName = reader.GetString(4);
                    user.LastName = reader.GetString(5);
                    user.Bio = reader.GetString(6);
                }
            }

            return user;
        }

        public User? GetUserByName(string username)
        {
            User user = null;

            var cmd = new NpgsqlCommand("SELECT * FROM game_user WHERE name=$1", _connection)
            {
                Parameters =
                    {
                        new() { Value = username }
                    }
            };

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    user = new User(new UserLoginData { UserName = reader.GetString(0), Password = reader.GetString(1) });
                    user.Coins = reader.GetInt32(2);
                    user.Score = reader.GetInt32(3);
                    user.FirstName = reader.GetString(4);
                    user.LastName = reader.GetString(5);
                    user.Bio = reader.GetString(6);
                }
            }

            return user;
        }

        public bool AddUser(User newUser)
        {
            try
            {
                var cmd = new NpgsqlCommand("INSERT INTO game_user (name, password, coins, score, first_name, last_name, bio) VALUES ($1, $2, $3, $4, $5, $6, $7)", _connection)
                {
                    Parameters =
                    {
                        new() { Value = newUser.Name },
                        new() { Value = newUser.Password },
                        new() { Value = newUser.Coins },
                        new() { Value = newUser.Score },
                        new() { Value = String.Empty },
                        new() { Value = String.Empty },
                        new() { Value = String.Empty },
                    }
                };
                var result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex: ", ex);
                return false;
            }

            return true;
        }

        public bool UpdateUser(User user)
        {
            try
            {
                var cmd = new NpgsqlCommand("UPDATE game_user SET coins=$1, score=$2, first_name=$3, last_name=$4, bio=$5 WHERE name=$6", _connection)
                {
                    Parameters =
                    {
                        new() { Value = user.Coins },
                        new() { Value = user.Score },
                        new() { Value = user.FirstName },
                        new() { Value = user.LastName },
                        new() { Value = user.Bio },
                        new() { Value = user.Name },
                    }
                };
                var result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public List<string> GetUserScores()
        {
            List<String> scores = new List<String>();

            var cmd = new NpgsqlCommand("SELECT * FROM game_user", _connection);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    if (name.ToUpper() != "ADMIN")
                    {
                        scores.Add(reader.GetString(0) + ": " + reader.GetInt32(3));
                    }
                }
            }

            return scores;
        }

        #endregion


        #region manage game functions
        public bool AddPackage(Package package)
        {
            try
            {
                //Add package Id
                Random rnd = new Random();
                var id = rnd.Next(1, 999999999);
                var cmd = new NpgsqlCommand("INSERT INTO package (id) VALUES ($1)", _connection)
                {
                    Parameters =
                    {
                        new() { Value = id }
                    }
                };
                cmd.ExecuteNonQuery();

                //Add cards with package id
                foreach (var card in package.Cards)
                {
                    cmd = new NpgsqlCommand("INSERT INTO card (id, name, damage, elementtype, cardtype, package_id) VALUES ($1, $2, $3, $4, $5, $6)", _connection)
                    {
                        Parameters =
                        {
                            new() { Value = card.Id },
                            new() { Value = card.Name },
                            new() { Value = card.Damage },
                            new() { Value = (int)card.ElementType },
                            new() { Value = (int)card.CardType },
                            new() { Value = id }
                        }
                    };

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex: ", ex);
                return false;
            }

            return true;
        }

        public bool BuyPackage(User user)
        {
            try
            {
                //Get first package
                var cmd = new NpgsqlCommand("SELECT * FROM package WHERE deleted ISNULL LIMIT 1", _connection);
                int packageId = 0;
                List<string> cards = new List<string>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        packageId = reader.GetInt32(0);
                    }
                }

                if (packageId == 0)
                {
                    Console.WriteLine("No packages found");
                    return false;
                }

                //set update cards in package -> set user name
                cmd = new NpgsqlCommand("UPDATE card SET username=$1 WHERE package_id=$2", _connection)
                {
                    Parameters =
                    {
                        new() { Value = user.Name },
                        new() { Value = packageId }
                    }
                };
                cmd.ExecuteNonQuery();

                //Remove acuired package
                cmd = new NpgsqlCommand("UPDATE package SET deleted=1 WHERE id=" + packageId, _connection);
                cmd.ExecuteNonQuery();

                //update user coins
                user.Coins -= 5;
                UpdateUser(user);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public List<Card> GetCards(User user)
        {
            var cards = new List<Card>();

            try
            {
                var cmd = new NpgsqlCommand("SELECT * FROM card WHERE username=$1", _connection)
                {
                    Parameters =
                    {
                        new() { Value = user.Name }
                    }
                };

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var card = new Card(reader.GetString(0), reader.GetString(1), reader.GetInt32(2).ToString());
                        card.ElementType = (ElementType)reader.GetInt32(3);
                        card.CardType = (CardType)reader.GetInt32(4);
                        cards.Add(card);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting cards");
            }

            return cards;
        }

        public bool ConfigureDeck(User user, Deck deck)
        {
            try
            {
                Random rnd = new Random();
                var randomId = rnd.Next(1, 999999999).ToString();

                foreach (var card in deck.Cards)
                {
                    var cmd = new NpgsqlCommand("UPDATE card SET deck_id=$1 WHERE id=$2", _connection)
                    {
                        Parameters =
                        {
                            new() { Value = randomId },
                            new() { Value = card.Id },
                        }
                    };

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public List<Deck> GetDecks(User user)
        {
            List<string> deckids = new List<string>();
            List<Deck> decks = new List<Deck>();

            try
            {
                // get all deckids belonging to user
                var cmd = new NpgsqlCommand("SELECT deck_id FROM CARD WHERE username=$1 AND deck_id NOTNULL", _connection)
                {
                    Parameters =
                    {
                        new() { Value = user.Name }
                    }
                };

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!deckids.Contains(reader.GetString(0)))
                        {
                            deckids.Add(reader.GetString(0));
                        }
                    }
                }

                //build deck list
                foreach (var deckId in deckids)
                {
                    cmd = new NpgsqlCommand("SELECT * FROM CARD WHERE deck_id=$1", _connection)
                    {
                        Parameters =
                        {
                            new() { Value = deckId }
                        }
                    };

                    Card[] cards = new Card[4];
                    int index = 0;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var card = new Card(reader.GetString(0), reader.GetString(1), reader.GetInt32(2).ToString());
                            card.ElementType = (ElementType)reader.GetInt32(3);
                            card.CardType = (CardType)reader.GetInt32(4);
                            cards[index] = card;
                            index++;
                        }
                    }

                    decks.Add(new Deck(cards[0], cards[1], cards[2], cards[3]));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return decks;
        }


        #endregion

    }
}
