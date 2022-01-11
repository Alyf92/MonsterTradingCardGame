using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonsterCard;
using Xunit.Sdk;

namespace MonsterTest
{
    [TestClass]
    public class BusinessLayerTest
    {
        [TestMethod]
        public void CreateBusinessLayer_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            Assert.IsNotNull(bl);
        }

        [TestMethod]
        public void RegisterUser_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            var result = bl.RegisterUser(new UserLoginData { UserName = "Test1", Password = "Test1" });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RegisterUser_Failed()
        {
            BusinessLayer bl = new BusinessLayer();
            var result = bl.RegisterUser(new UserLoginData { UserName = "Test1", Password = "Test1" });
            result = bl.RegisterUser(new UserLoginData { UserName = "Test1", Password = "Test1" });

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RegisterUser_Throw_Exeption()
        {
            BusinessLayer bl = new BusinessLayer();
            Assert.ThrowsException<System.ArgumentNullException>(() => bl.RegisterUser(new UserLoginData { UserName = null, Password = null }));
        }

        [TestMethod]
        public void LoginUser_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "Test2", Password = "Test2" });

            var token = bl.Login(new UserLoginData { UserName = "Test2", Password = "Test2" });
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void LoginUser_Throw_Exception()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "Test3", Password = "Test3" });

            Assert.ThrowsException<System.ArgumentNullException>(() => bl.Login(new UserLoginData { UserName = null, Password = null }));
        }

        [TestMethod]
        public void LoginUser_Failed()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "Test4", Password = "Test4" });

            var token = bl.Login(new UserLoginData { UserName = "Test0", Password = "Test0" });
            Assert.IsNull(token);
        }

        [TestMethod]
        public void GenerateToken_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            var token = bl.GenerateToken("firstname", "lastname");
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void GetUserFromToken_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            var token = bl.GenerateToken("name", "password");
            var user = bl.GetUserFromToken(token);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void GetUserFromToken_Failed()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            var token = bl.GenerateToken("name", "password");
            token = token.Substring(0, token.Length - 4);
            token += "1234";
            var user = bl.GetUserFromToken(token);
            Assert.IsNull(user);
        }


        [TestMethod]
        public void AddPackage_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            var token = "L055bnNrQUJaVjZ5ZzhjQ1BIWjUxRkJZYmQvWnhTcTVYNGl2UVNnZkxUcz06QWRtaW4=";

            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);
            var result = bl.AddPackage(package, token);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddPackage_Throw_ArgumentNullException()
        {
            BusinessLayer bl = new BusinessLayer();
            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);

            Assert.ThrowsException<System.ArgumentNullException>(() => bl.AddPackage(package, null));
        }

        [TestMethod]
        public void AddPackage_Throw_UnauthorizedAccessException()
        {
            BusinessLayer bl = new BusinessLayer();
            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);

            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            var token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });

            Assert.ThrowsException<System.UnauthorizedAccessException>(() => bl.AddPackage(package, token));
        }


        [TestMethod]
        public void BuyPackage_Success()
        {
            BusinessLayer bl = new BusinessLayer();

            //Add
            var token = "L055bnNrQUJaVjZ5ZzhjQ1BIWjUxRkJZYmQvWnhTcTVYNGl2UVNnZkxUcz06QWRtaW4=";

            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);
            bl.AddPackage(package, token);

            //Buy
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            var result = bl.BuyPackage(token);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BuyPackage_Throw_ArgumentNullException()
        {
            BusinessLayer bl = new BusinessLayer();
            Assert.ThrowsException<System.ArgumentNullException>(() => bl.BuyPackage(null));
        }

        [TestMethod]
        public void BuyPackage_Throw_UnauthorizedAccessException()
        {
            BusinessLayer bl = new BusinessLayer();

            //Add
            var token = "L055bnNrQUJaVjZ5ZzhjQ1BIWjUxRkJZYmQvWnhTcTVYNGl2UVNnZkxUcz06QWRtaW4=";

            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);
            bl.AddPackage(package, token);

            //Buy
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            token = token.Substring(0, token.Length - 4);
            token += "1234";

            Assert.ThrowsException<System.UnauthorizedAccessException>(() => bl.BuyPackage(token));
        }

        [TestMethod]
        public void BuyPackage_Failed_NoCoins()
        {
            BusinessLayer bl = new BusinessLayer();

            //Add
            var token = "L055bnNrQUJaVjZ5ZzhjQ1BIWjUxRkJZYmQvWnhTcTVYNGl2UVNnZkxUcz06QWRtaW4=";

            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);
            bl.AddPackage(package, token);

            //Buy
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            bl.GetUserFromToken(token).Coins = 4; ;
            var result = bl.BuyPackage(token);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetCards_Success()
        {
            BusinessLayer bl = new BusinessLayer();

            //Add
            var token = "L055bnNrQUJaVjZ5ZzhjQ1BIWjUxRkJZYmQvWnhTcTVYNGl2UVNnZkxUcz06QWRtaW4=";

            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);
            bl.AddPackage(package, token);

            //Buy
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            bl.BuyPackage(token);
            var result = bl.GetCards(token);

            Assert.AreEqual(result[0].Name, "test1");
        }

        [TestMethod]
        public void GetCards_Throw_ArgumentNullException()
        {
            BusinessLayer bl = new BusinessLayer();
            Assert.ThrowsException<System.ArgumentNullException>(() => bl.GetCards(null));
        }

        [TestMethod]
        public void GetCards_Throw_UnauthorizedAccessException()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            var token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            token = token.Substring(0, token.Length - 4);
            token += "1234";

            Assert.ThrowsException<System.UnauthorizedAccessException>(() => bl.GetCards(token));
        }

        [TestMethod]
        public void ConfigureDeck_Success()
        {
            BusinessLayer bl = new BusinessLayer();

            //Add Packages
            var token = "L055bnNrQUJaVjZ5ZzhjQ1BIWjUxRkJZYmQvWnhTcTVYNGl2UVNnZkxUcz06QWRtaW4=";

            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);
            bl.AddPackage(package, token);

            Card[] card2 = { new Card("22", "test22", "10.0"), new Card("33", "test33", "10.0"), new Card("44", "test44", "14.0"), new Card("55", "test55", "8.0"), new Card("66", "test66", "40.0") };
            package = new Package(card2);
            bl.AddPackage(package, token);

            //Buy Packages
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            bl.BuyPackage(token);
            bl.BuyPackage(token);

            //Configure Deck
            string[] ids = { "22", "4", "1", "5" };

            var result = bl.ConfigureDeck(ids, token);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ConfigureDeck_Throw_ArgumentNullException()
        {
            BusinessLayer bl = new BusinessLayer();
            string[] ids = { "22", "4", "1", "5" };

            Assert.ThrowsException<System.ArgumentNullException>(() => bl.ConfigureDeck(ids, null));
        }

        [TestMethod]
        public void ConfigureDeck_Throw_UnauthorizedAccessException()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            var token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            token = token.Substring(0, token.Length - 4);
            token += "1234";
            string[] ids = { "22", "4", "1", "5" };

            Assert.ThrowsException<System.UnauthorizedAccessException>(() => bl.ConfigureDeck(ids, token));
        }

        [TestMethod]
        public void ConfigureDeck_Throw_ArgumentException_WrongNumOfCard()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            var token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            string[] ids = { "22", "4", "1" };

            Assert.ThrowsException<System.ArgumentException>(() => bl.ConfigureDeck(ids, token));
        }


        [TestMethod]
        public void GetDecks_Success()
        {
            BusinessLayer bl = new BusinessLayer();

            //Add Packages
            var token = "L055bnNrQUJaVjZ5ZzhjQ1BIWjUxRkJZYmQvWnhTcTVYNGl2UVNnZkxUcz06QWRtaW4=";

            Card[] card = { new Card("1", "test1", "10.0"), new Card("4", "test1", "10.0"), new Card("2", "test5", "14.0"), new Card("t", "test5", "8.0"), new Card("5", "test2", "40.0") };
            var package = new Package(card);
            bl.AddPackage(package, token);

            Card[] card2 = { new Card("22", "test22", "10.0"), new Card("33", "test33", "10.0"), new Card("44", "test44", "14.0"), new Card("55", "test55", "8.0"), new Card("66", "test66", "40.0") };
            package = new Package(card2);
            bl.AddPackage(package, token);

            //Buy Packages
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            bl.BuyPackage(token);
            bl.BuyPackage(token);

            //Configure Deck
            string[] ids = { "22", "4", "1", "5" };
            bl.ConfigureDeck(ids, token);

            //Get Deck
            var result = bl.GetDecks(token);


            Assert.AreEqual(result[0].Cards[0].Name, "test22");
        }

        [TestMethod]
        public void GetScore_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "name", Password = "password" });
            var token = bl.Login(new UserLoginData { UserName = "name", Password = "password" });
            var user = bl.GetUserFromToken(token);
            user.Score = 55;

            var result = bl.GetScore(token);

            Assert.AreEqual(result, 55);
        }

        [TestMethod]
        public void GetScoreBoard_Success()
        {
            BusinessLayer bl = new BusinessLayer();
            bl.RegisterUser(new UserLoginData { UserName = "user1", Password = "password1" });
            bl.RegisterUser(new UserLoginData { UserName = "user2", Password = "password2" });

            var token1 = bl.Login(new UserLoginData { UserName = "user1", Password = "password1" });
            var token2 = bl.Login(new UserLoginData { UserName = "user2", Password = "password2" });

            var user1 = bl.GetUserFromToken(token1);
            var user2 = bl.GetUserFromToken(token2);
            user1.Score = 88;
            user2.Score = 50;

            var result = bl.GetScoreBoard(token1);

            Assert.AreEqual(result[0], "user1: 88");
            Assert.AreEqual(result[1], "user2: 50");
        }



    }
}