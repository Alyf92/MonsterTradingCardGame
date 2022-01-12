using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace MonsterCard
{
    public class Program
    {

        private static HttpListener _listener;
        private static string _url = "http://localhost:8000/";
        private static readonly BusinessLayer _businessLayer = new BusinessLayer();

        public static async Task HandleIncomingConnections()
        {
            while (true)
            {
                ThreadPool.QueueUserWorkItem(Process, _listener.GetContext());
            }
        }

        public static void Process(object o)
        {
            var ctx = o as HttpListenerContext;

            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;
            var responseData = "";
            var statusCode = 0;
            var receivedToken = req.Headers.Get("Authorization");
            var requestData = GetRequestData(req);
            var method = req.HttpMethod;

            var pathParts = req.Url.AbsolutePath.Split(new string[] { "/" }, StringSplitOptions.None).Skip(1).ToArray();


            switch (pathParts[0])
            {
                case "users":
                    if (pathParts.Length < 2)
                    {
                        if (requestData.Length == 0)
                        {
                            responseData = "ERROR";
                            statusCode = 400;
                        }
                        else
                        {
                            var loginData = JsonConvert.DeserializeObject<UserLoginData>(requestData);
                            bool result = false;

                            try
                            {
                                result = _businessLayer.RegisterUser(loginData);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                responseData = result ? "OK" : "ERROR";
                                statusCode = result ? 200 : 400;
                            }
                        }
                    }
                    else
                    {
                        if (method == "GET")
                        {
                            UserData userData = null;

                            try
                            {
                                userData = _businessLayer.GetUserData(pathParts[1], receivedToken);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                responseData = userData != null ? JsonConvert.SerializeObject(userData) : "ERROR";
                                statusCode = userData == null ? 200 : 400;
                            }
                        }

                        if (method == "PUT")
                        {
                            var updateResult = false;

                            try
                            {
                                var userData = JsonConvert.DeserializeObject<UserData>(requestData);
                                updateResult = _businessLayer.UpdateUserData(userData, pathParts[1], receivedToken);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                responseData = updateResult ? "OK" : "ERROR";
                                statusCode = updateResult ? 200 : 400;
                            }
                        }
                    }
                    break;
                case "sessions":
                    if (requestData.Length == 0)
                    {
                        responseData = "ERROR";
                        statusCode = 400;
                    }
                    else
                    {
                        var loginData = JsonConvert.DeserializeObject<UserLoginData>(requestData);
                        string token = "";

                        try
                        {
                            token = _businessLayer.Login(loginData);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            responseData = token != null ? token : "ERROR";
                            statusCode = token != null ? 200 : 400;
                        }
                    }
                    break;
                case "packages":
                    if (requestData.Length == 0)
                    {
                        responseData = "ERROR";
                        statusCode = 400;
                    }
                    else
                    {
                        var added = false;

                        try
                        {
                            var cards = JsonConvert.DeserializeObject<Card[]>(requestData);

                            if (cards != null)
                            {
                                added = _businessLayer.AddPackage(new Package(cards), receivedToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            responseData = added == true ? "OK" : "ERROR";
                            statusCode = added == true ? 200 : 400;
                        }
                    }
                    break;
                case "transactions":
                    var bougt = false;

                    try
                    {
                        bougt = _businessLayer.BuyPackage(receivedToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        responseData = bougt == true ? "OK" : "ERROR";
                        statusCode = bougt == true ? 200 : 400;
                    }
                    break;
                case "cards":
                    var userCards = new List<Card>();
                    var success = false;

                    try
                    {
                        userCards = _businessLayer.GetCards(receivedToken);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        responseData = success == true ? JsonConvert.SerializeObject(userCards) : "ERROR";
                        statusCode = success == true ? 200 : 400;
                    }
                    break;
                case "deck":
                    var handelDeckSuccess = false;
                    List<Deck> decks = new List<Deck>();

                    try
                    {
                        if (method == "GET")
                        {
                            decks = _businessLayer.GetDecks(receivedToken);
                            handelDeckSuccess = true;
                        }
                        else if (method == "PUT")
                        {
                            var cardIds = JsonConvert.DeserializeObject<String[]>(requestData);
                            handelDeckSuccess = _businessLayer.ConfigureDeck(cardIds, receivedToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        statusCode = handelDeckSuccess ? 200 : 400;
                        responseData = handelDeckSuccess == true ? (method == "GET" ? JsonConvert.SerializeObject(decks) : "OK") : "ERROR";
                    }
                    break;
                case "stats":
                    try
                    {
                        var stats = _businessLayer.GetScore(receivedToken);
                        statusCode = 200;
                        responseData = "Your score: " + stats;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        statusCode = 400;
                        responseData = "ERROR";
                    }
                    break;
                case "score":
                    try
                    {
                        var scoreBoard = _businessLayer.GetScoreBoard(receivedToken);
                        statusCode = 200;
                        responseData = JsonConvert.SerializeObject(scoreBoard);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        statusCode = 400;
                        responseData = "ERROR";
                    }
                    break;
                case "battles":
                    try
                    {
                        var battleResult = _businessLayer.AddUserToBattle(receivedToken, Int32.Parse(requestData));
                        statusCode = 200;
                        responseData = JsonConvert.SerializeObject(battleResult);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        statusCode = 400;
                        responseData = "ERROR";
                    }
                    break;
                default:
                    statusCode = 400;
                    responseData = "ERROR! NOT Found";
                    break;

            }


            // Write the response info
            byte[] data = Encoding.UTF8.GetBytes(responseData);
            resp.ContentType = "text/html";
            resp.StatusCode = statusCode;
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }

        public static string GetRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return "";
            }

            var body = request.InputStream;
            var encoding = request.ContentEncoding;
            var reader = new StreamReader(body, encoding);
            var data = reader.ReadToEnd();

            body.Close();
            reader.Close();
            return data;
        }

        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            _listener = new HttpListener();
            _listener.Prefixes.Add(_url);
            _listener.Start();
            Console.WriteLine("Listening for connections on {0}", _url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            _listener.Close();
        }
    }

}