using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace ExoHttpAPI
{
    internal class Program
    {

        private static bool serverEnabled = false;
        private static int serverPort = 8080;
        private static string TMDB_Film_List;
        DatabaseInterface bdd = new DatabaseInterface();

        static void Main(string[] args)
        {
            Log("Bienvenue sur le serveur d'API de film NetFilm.");
            Log("Projet réalisé par François SAURA et Loïc LABAISSE");
            Log("");
            TMDB_Film_List = GetJsonApi();
            serverEnabled = true;
            ReadRequest();
        }

        static void ReadRequest()
        {
            var listenner = new HttpListener();
            listenner.Prefixes.Add("http://*:" + serverPort + "/");
            //listenner.AuthenticationSchemes = AuthenticationSchemes.Basic; //A TESTER
            
            try {
                listenner.Start();
            }
            catch {
                Log("(X) Impossible de démarrer la serveur. Essayez de redémarrer l'application en tant qu'administrateur ou vérifiez que le port " + serverPort + " n'est pas déjà utilisé par un autre processus.");
                return;
            }

            Log("Serveur démarré sur le port " + serverPort);

            while (serverEnabled)
            {
                HttpListenerContext conn = listenner.GetContext();
                string body = null;
                StreamReader sr = new StreamReader(conn.Request.InputStream);
                body = sr.ReadToEnd();
                sr.Close();
                Log("Requête reçu par " + conn.Request.RemoteEndPoint.ToString() + "(" + conn.Request.RawUrl + ") body = '" + body + "'");

                //Log("Requête reçu par " + conn.Request.RemoteEndPoint.ToString() + "(" + conn.Request.RawUrl + ")");
                //ExecuteRequest(conn);
                ExecuteRequest(conn, body);
            }

            listenner.Stop();

        }

        static void ExecuteRequest(HttpListenerContext conn, string body = "")
        {
            //string Route = conn.Request.RawUrl; //Ancienne méthode

            //Tester cette méthode pour décoder les caractères spéciaux tel que é, è, ç, ï, etc...
            string Route = System.Web.HttpUtility.UrlDecode(conn.Request.RawUrl, System.Text.Encoding.UTF8); //   /!\ A tester sur le front avec axios

            if (conn.Request.RawUrl == "/") { //Route principale (index)
                SendHtmlResponse(conn, "<html><body><h1>Projet Site de films</h1><h2><u>Matiere:</u> Service WEB: Communication et echange de donnees</h2><h2><u>Participants:</u> Francois SAURA et Loic LABAISSE</h2><h2><u>Objectifs:</u> Developper un site internet d informations cinematographique en utilisant l API de TheMovieDB</h2><h1 style='text-align: center;'>BIENVENUE SUR LE SERVEUR API (Backend)</h1><p style='text-align: center;'>Veuillez utiliser convenablement l API avec une URI valide.</p></body></html>");
                return;
            }

            string[] Path = Route.Split("/");

            //Récupération du body de la requête:
            //string body = null;
            //StreamReader sr;
            //try
            //{
            //    sr = new StreamReader(conn.Request.InputStream);
            //    body = sr.ReadToEnd();
            //    sr.Close();
            //}
            //catch
            //{
            //    Log("Le body de la requête " + Route + " n'a pas pu être récupéré !");
            //}

            if (Path[1].ToUpper() == "FILMS") { //Route /FILMS
                if (Path.Length == 2)
                {
                    //string jsonString = GetJsonApi();
                    //SendJsonResponse(conn, jsonString);
                    SendJsonResponse(conn, TMDB_Film_List);
                }
                else if (Path.Length >= 3)
                {
                    string userID = Path[2];
                    int userID_int = -1;
                    try
                    {
                        userID_int = Convert.ToInt32(userID);
                    } catch
                    {
                        var ErrorJson = new GetResponse();
                        ErrorJson.comment = "L'ID de l'utilisateur fournie dans la route n'est pas un entier." + Environment.NewLine + "Voici la route incorrecte que vous avez entrer:" + Environment.NewLine + Route;
                        ErrorJson.statusCode = 400;
                        SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                        return;
                    }
                    var movieResponse = new DiscoverResponse();
                    if (movieResponse.MakeFromExistingApiCall(TMDB_Film_List, Convert.ToInt32(userID_int)) == false)
                    {
                        var ErrorJson = new GetResponse();
                        ErrorJson.comment = "Erreur de traitement de l'API. Les données des films provenant de l'API TheMovieDB chargé au démarrage ne doivent pas être présent ou correcte.";
                        ErrorJson.statusCode = 500;
                        SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                        return;
                    }
                    string jsonRep = movieResponse.getJsonString();
                    SendJsonResponse(conn, jsonRep, 200);

                }



            } else if (Path[1].ToUpper() == "LOGIN") { //Route /LOGIN
                if (Path.Length < 4)
                {
                    LoginResponse ErrorResponseObj = new LoginResponse();
                    ErrorResponseObj.ErrorComment = "Mauvaise utilisation de l'API. Vérifiez votre route. Il doit y avoir au moins 1 autres slash après la route /Login/ (Domain:Port/Login/email/passhash)";
                    SendJsonResponse(conn, ErrorResponseObj.getJsonResponse(), 400);
                    return;
                }
                string Email = Path[2];
                string Passhash = Path[3];

                LoginResponse RepObj = new LoginResponse(Email, Passhash);

                String jsonRepString = JsonConvert.SerializeObject(RepObj);
                if (RepObj.exist == true)
                {
                    SendJsonResponse(conn, jsonRepString, 200);
                    Log("Utilisateur " + RepObj.first_name + " " + RepObj.last_name + " Authentifié avec succès !");
                } else
                {
                    SendJsonResponse(conn, jsonRepString, 400); //L'utilisateur est introuvable ou le login/mdp ne correspond pas.
                    Log("Tentative d'identification de l'utilisateur " + Email + " échouée.");
                }


            } else if (Path[1].ToUpper() == "REGISTER") { //Route /REGISTER
                var repObj = new GetResponse();

                if (Path.Length < 6)
                {
                    repObj.comment = "Mauvaise utilisation de l'API. Vérifiez votre route. Il doit y avoir au moins 3 autres slash après la route /REGISTER/ (Domain:Port/Login/first_name/last_name/email/passhash)";
                    repObj.statusCode = 400;
                    SendJsonResponse(conn, repObj.getJsonResponse(), repObj.statusCode);
                    return;
                }

                string first_name = Path[2];
                string last_name = Path[3];
                string email = Path[4];
                string passhash = Path[5];

                Log("Demande de création de l'utilisateur " + first_name + " " + last_name + " avec l'email: " + email + " et le mot de passe hashé: " + passhash);


                //Vérifie que l'email n'est pas déjà existant dans la base:
                var bdd = new DatabaseInterface();

                MySqlDataReader userExistReader = bdd.SELECT("SELECT id FROM users WHERE email='" + email + "';");
                if (userExistReader == null) {
                    //SendStatutResponse(conn, 400); //Ancienne méthode ( /!\ tester la nouvelle avec l'envoi de repObj)
                    repObj.statusCode = 400;
                    repObj.comment = "Impossible de se connecter à la base de donnée pour vérifier si l'utilisateur à créer n'existe pas déjà";
                    SendJsonResponse(conn, repObj.getJsonResponse(), 400);
                    return;
                }
                if (userExistReader.HasRows) {
                    repObj.statusCode = 400;
                    repObj.comment = "L'utilisateur existe déjà en base de donnée. Cette adresse e-mail est déjà utilisé par un autre utilisateur.";
                    SendJsonResponse(conn, repObj.getJsonResponse(), 400);
                    userExistReader.Close();
                    return;
                }

                //On insère le nouvel utilisateur dans la base de donnée:
                bool insertResult = bdd.INSERT_INTO("INSERT INTO users (prenom, nom, email, password) VALUES ('" + first_name + "', '" + last_name + "', '" + email + "', '" + passhash + "');");

                if (insertResult == true) {
                    //SendStatutResponse(conn, 200);
                    repObj.statusCode = 200;
                    repObj.comment = "Utilisateur créer avec succès.";
                    SendJsonResponse(conn, repObj.getJsonResponse());
                    Log("L'utilisateur à bien été créer en base de donnée.");
                } else {
                    repObj.statusCode = 400;
                    repObj.comment = "Impossible d'ajouter l'utilisateur dans la base de donnée.";
                    //SendStatutResponse(conn, 400);
                    SendJsonResponse(conn, repObj.getJsonResponse(), 400);
                    Log("Impossible d'effectuer une requête d'insertion en base de donnée pour ajouter l'utilisateur.");
                }

            } else if (Path[1].ToUpper() == "SETLIKE") // ROUTE SETLIKE
            {
                string userID = Path[2];
                string filmID = Path[3];

                int userID_int = -1;
                try { userID_int = Convert.ToInt32(userID); }
                catch {
                    var ErrorJson = new GetResponse();
                    ErrorJson.comment = "L'ID de l'utilisateur fournie dans la route n'est pas un entier." + Environment.NewLine + "Voici la route incorrecte que vous avez entrer:" + Environment.NewLine + Route;
                    ErrorJson.statusCode = 400;
                    SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                    return;
                }

                var bdd = new DatabaseInterface();
                string select_req = "SELECT id FROM likes WHERE user_id = '" + userID + "' AND film_id = '" + filmID + "'";
                MySqlDataReader likeExistReader = bdd.SELECT(select_req);
                bool alreadyLiked = false;
                alreadyLiked = likeExistReader.HasRows;
                if (alreadyLiked == true)
                {
                    var repObj = new GetResponse();
                    repObj.statusCode = 200;
                    repObj.comment = "L'utilisateur à déjà aimer ce film.";
                    SendJsonResponse(conn, repObj.getJsonResponse(), repObj.statusCode);
                    return;
                }
                string insert_req = "INSERT INTO likes (user_id, film_id) VALUES ('" + userID + "', '" + filmID + "');";
                bool likeConfirmation = bdd.INSERT_INTO(insert_req);
                if (likeConfirmation)
                {
                    //var repObj = new GetResponse();
                    //repObj.statusCode=200;
                    //repObj.comment = "La mention j'aime à été ajoutée avec succès !";
                    //SendJsonResponse(conn, repObj.getJsonResponse(), repObj.statusCode);
                    //return;

                    //Loic veux que je lui retourne le JSON avec le like updaté (pour lui éviter de refaire un /FILMS/<userID>)
                    var movieResponse = new DiscoverResponse();
                    if (movieResponse.MakeFromExistingApiCall(TMDB_Film_List, Convert.ToInt32(userID_int)) == false)
                    {
                        var ErrorJson = new GetResponse();
                        ErrorJson.comment = "Erreur de traitement de l'API. Les données des films provenant de l'API TheMovieDB chargé au démarrage ne doivent pas être présent ou correcte.";
                        ErrorJson.statusCode = 500;
                        SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                        return;
                    }
                    string jsonRep = movieResponse.getJsonString();
                    SendJsonResponse(conn, jsonRep, 200);

                    return;
                } else
                {
                    var repObj = new GetResponse();
                    repObj.statusCode = 400;
                    repObj.comment = "Une erreur est surveue durant l'insertion de la mention j'aime dans la base de donnée.";
                    SendJsonResponse(conn, repObj.getJsonResponse(), repObj.statusCode);
                    return;
                }


            } else if (Path[1].ToUpper() == "UNSETLIKE") { //ROUTE UNSETLIKE
                string userID = Path[2];
                string filmID = Path[3];

                int userID_int = -1;
                try { userID_int = Convert.ToInt32(userID); }
                catch
                {
                    var ErrorJson = new GetResponse();
                    ErrorJson.comment = "L'ID de l'utilisateur fournie dans la route n'est pas un entier." + Environment.NewLine + "Voici la route incorrecte que vous avez entrer:" + Environment.NewLine + Route;
                    ErrorJson.statusCode = 400;
                    SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                    return;
                }


                var bdd = new DatabaseInterface();
                string delete_req = "DELETE FROM likes WHERE user_id = '" + userID + "' AND film_id = '" + filmID + "';";
                if (bdd.INSERT_INTO(delete_req))
                {
                    //var repObj = new GetResponse();
                    //repObj.statusCode=200;
                    //repObj.comment = "Le mention j'aime à bien été retirée.";
                    //SendJsonResponse(conn, repObj.getJsonResponse(), repObj.statusCode);
                    //return;

                    //Loic veut que j'envois le JSON contenant la liste des films de l'utilisateur (avec ces likes) en tant que réponse:
                    var movieResponse = new DiscoverResponse();
                    if (movieResponse.MakeFromExistingApiCall(TMDB_Film_List, Convert.ToInt32(userID_int)) == false)
                    {
                        var ErrorJson = new GetResponse();
                        ErrorJson.comment = "Erreur de traitement de l'API. Les données des films provenant de l'API TheMovieDB chargé au démarrage ne doivent pas être présent ou correcte.";
                        ErrorJson.statusCode = 500;
                        SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                        return;
                    }
                    string jsonRep = movieResponse.getJsonString();
                    SendJsonResponse(conn, jsonRep, 200);
                    return;
                } else
                {
                    var repObj = new GetResponse();
                    repObj.statusCode = 400;
                    repObj.comment = "La requête de suppression de la mention j'aime ne s'est pas déroulé correctement.";
                    SendJsonResponse(conn, repObj.getJsonResponse(), repObj.statusCode);
                    return;
                }

            } else if (Path[1].ToUpper() == "LIKE") //ROUTE LIKE
            {
                var userID = Path[2];
                string filmID = Path[3];
                string liked = Path[4];

                int userID_int = -1;
                try { userID_int = Convert.ToInt32(userID); }
                catch
                {
                    var ErrorJson = new GetResponse();
                    ErrorJson.comment = "L'ID de l'utilisateur fournie dans la route n'est pas un entier." + Environment.NewLine + "Voici la route incorrecte que vous avez entrer:" + Environment.NewLine + Route;
                    ErrorJson.statusCode = 400;
                    SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                    return;
                }

                //Console.WriteLine("userID: '"+userID+"', filmID: '"+filmID+"', liked: '"+liked+"'");

                if (liked.ToUpper() == "TRUE")
                {
                    var bdd = new DatabaseInterface();
                    string select_req = "SELECT id FROM likes WHERE user_id = '" + userID + "' AND film_id = '" + filmID + "'";
                    MySqlDataReader likeExistReader = bdd.SELECT(select_req);
                    bool alreadyLiked = false;
                    alreadyLiked = likeExistReader.HasRows;
                    if (alreadyLiked == true)
                    {
                        //Loic veut que j'envois le JSON contenant la liste des films de l'utilisateur (avec ces likes) en tant que réponse:
                        var movieResponse = new DiscoverResponse();
                        if (movieResponse.MakeFromExistingApiCall(TMDB_Film_List, Convert.ToInt32(userID_int)) == false)
                        {
                            var ErrorJson = new GetResponse();
                            ErrorJson.comment = "Erreur de traitement de l'API. Les données des films provenant de l'API TheMovieDB chargé au démarrage ne doivent pas être présent ou correcte.";
                            ErrorJson.statusCode = 500;
                            SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                            return;
                        }
                        string jsonRep = movieResponse.getJsonString();
                        SendJsonResponse(conn, jsonRep, 200);
                        return;
                    }
                    string insert_req = "INSERT INTO likes (user_id, film_id) VALUES ('" + userID + "', '" + filmID + "');";
                    bool likeConfirmation = bdd.INSERT_INTO(insert_req);
                    if (likeConfirmation)
                    {
                        //Loic veut que j'envois le JSON contenant la liste des films de l'utilisateur (avec ces likes) en tant que réponse:
                        var movieResponse = new DiscoverResponse();
                        if (movieResponse.MakeFromExistingApiCall(TMDB_Film_List, Convert.ToInt32(userID_int)) == false)
                        {
                            var ErrorJson = new GetResponse();
                            ErrorJson.comment = "Erreur de traitement de l'API. Les données des films provenant de l'API TheMovieDB chargé au démarrage ne doivent pas être présent ou correcte.";
                            ErrorJson.statusCode = 500;
                            SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                            return;
                        }
                        string jsonRep = movieResponse.getJsonString();
                        SendJsonResponse(conn, jsonRep, 200);
                        return;
                    }
                    else
                    {
                        var repObj4 = new GetResponse();
                        repObj4.statusCode = 500;
                        repObj4.comment = "Une erreur est surveue durant l'insertion de la mention j'aime dans la base de donnée.";
                        SendJsonResponse(conn, repObj4.getJsonResponse(), repObj4.statusCode);
                        return;
                    }
                } else if (liked.ToUpper() == "FALSE")
                {
                    var bdd = new DatabaseInterface();
                    string delete_req = "DELETE FROM likes WHERE user_id = '" + userID + "' AND film_id = '" + filmID + "';";
                    if (bdd.INSERT_INTO(delete_req))
                    {
                        //Loic veut que j'envois le JSON contenant la liste des films de l'utilisateur (avec ces likes) en tant que réponse:
                        var movieResponse = new DiscoverResponse();
                        if (movieResponse.MakeFromExistingApiCall(TMDB_Film_List, Convert.ToInt32(userID_int)) == false)
                        {
                            var ErrorJson = new GetResponse();
                            ErrorJson.comment = "Erreur de traitement de l'API. Les données des films provenant de l'API TheMovieDB chargé au démarrage ne doivent pas être présent ou correcte.";
                            ErrorJson.statusCode = 500;
                            SendJsonResponse(conn, ErrorJson.getJsonResponse(), ErrorJson.statusCode);
                            return;
                        }
                        string jsonRep = movieResponse.getJsonString();
                        SendJsonResponse(conn, jsonRep, 200);
                        return;
                    }
                    else
                    {
                        var repObj6 = new GetResponse();
                        repObj6.statusCode = 500;
                        repObj6.comment = "La requête de suppression de la mention j'aime ne s'est pas déroulé correctement.";
                        SendJsonResponse(conn, repObj6.getJsonResponse(), repObj6.statusCode);
                        return;
                    }
                }

            } else if (Path[1].ToUpper() == "POSTTEST") { //ROUTE POSTTEST
                GetResponse response = new GetResponse();
                response.statusCode = 200;
                response.comment = "Le post a du marcher";
                Console.WriteLine("Afficahge du body = '" + body + "'");
                SendJsonResponse(conn, response.getJsonResponse(), response.statusCode);
            } else if (Path[1].ToUpper() == "PUTLIKE") //ROUTE PUTLIKE
            {
                PutLikeResponse responseObj = new PutLikeResponse();
                GetResponse ErrorResponse = new GetResponse();
                if (responseObj.LoadRequest(body) == false)
                {
                    ErrorResponse.statusCode = 400;
                    ErrorResponse.comment = "Impossible de parser le JSON en backend que vous venez d'envoyer en tant que data du body.";
                    SendJsonResponse(conn, ErrorResponse.getJsonResponse(), ErrorResponse.statusCode);
                    return;
                }

                //responseObj.ReverseLike();

                if (responseObj.Execute() == false)
                {
                    ErrorResponse.statusCode = 500;
                    ErrorResponse.comment = "Impossible d'effectuer les opérations sur la base de donnée.";
                    SendJsonResponse(conn, ErrorResponse.getJsonResponse(), ErrorResponse.statusCode);
                    return;
                }

                SendJsonResponse(conn, responseObj.getJsonReponse(), 200);
                return;
            } else if (Path[1].ToUpper() == "POSTLOGIN") //ROUTE POSTLOGIN
            {
                JObject reqObj = JObject.Parse(body);
                GetResponse errorRepObj = new GetResponse();
                string email = null;
                string passhash = null;
                try
                {
                    email = reqObj.GetValue("email").ToString();
                    passhash = reqObj.GetValue("password").ToString();
                } catch
                {
                    errorRepObj.statusCode = 400;
                    errorRepObj.comment = "La récupération des clées dans votre JSON n'a pas été possible. Vérifiez que le JSON envoyé contient les clés que l'API doit reçevoir.";
                    SendJsonResponse(conn, errorRepObj.getJsonResponse(), errorRepObj.statusCode);
                    return;
                }
                LoginResponse repObj = new LoginResponse();
                SendJsonResponse(conn, repObj.getJsonResponse(), 200);

            } else if (Path[1].ToUpper() == "GETUSERLIKES")
            {
                UserLikesResponse RepObj = new UserLikesResponse();
                RepObj.Load(body, TMDB_Film_List);
                RepObj.GetLikes();
                SendJsonResponse(conn, RepObj.getJsonString(), RepObj.statusCode);
            }
            else {
                SendHtmlResponse(conn, Route + " : la route spécifié n'est pas définie.", 400);
            }


        }

        static void SendHtmlResponse(HttpListenerContext conn, string html, int statusCode = 200)
        {
            HttpListenerResponse rep = conn.Response;
            rep.ContentType = "text/html; charset=utf-8";
            rep.AppendHeader("Access-Control-Allow-Origin", "*");
            rep.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
            rep.AddHeader("Content-type", "application/json");
            rep.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT");
            rep.AddHeader("Access-Control-Max-Age", "1728000");
            rep.ContentEncoding = System.Text.Encoding.UTF8;
            rep.StatusCode = statusCode;
            byte[] htmlBytes = System.Text.Encoding.UTF8.GetBytes(html);
            rep.ContentLength64 = htmlBytes.Length;
            Stream outputStream = rep.OutputStream;
            outputStream.Write(htmlBytes, 0, htmlBytes.Length);
            outputStream.Close();
        }

        static bool SendJsonResponse(HttpListenerContext conn, string jsonString, int statusCode = 200)
        {
            HttpListenerResponse rep = conn.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonString); //Conversion du json qui va être retourné en binaire

            //Ajout des métadonnées de la réponse HTTP (headers):
            rep.ContentLength64 = buffer.Length;
            rep.ContentType = "application/json";
            rep.ContentEncoding = System.Text.Encoding.UTF8;
            //rep.AppendHeader("Access-Control-Allow-Origin", "*");
            rep.AddHeader("Access-Control-Allow-Origin", "*");
            rep.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
            rep.AddHeader("Content-type", "application/json");
            rep.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT");
            rep.AddHeader("Access-Control-Max-Age", "1728000");
            rep.StatusCode = statusCode;
            
            try
            {
                Stream outputStream = rep.OutputStream;
                outputStream.Write(buffer, 0, buffer.Length);
                outputStream.Close(); //Fermer le flux ici ? C'est surement en le fermant que le client peut faire son traitement.
                return true;
            } catch
            {
                Console.WriteLine(conn.Request.RemoteEndPoint.ToString()+" : Impossible de retourner la réponse JSON "+ statusCode + " car il y a un problème avec le flux sortant.");
                return false;
            }

        }

        static void SendStatutResponse(HttpListenerContext conn, int statutCode)
        {
            HttpListenerResponse rep = conn.Response;
            rep.AppendHeader("Access-Control-Allow-Origin", "*");
            rep.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
            rep.AddHeader("Content-type", "application/json");
            rep.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT");
            rep.AddHeader("Access-Control-Max-Age", "1728000");
            conn.Response.StatusCode = statutCode;

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("REPONSE CODE " + statutCode);
            Stream outputStream = rep.OutputStream;
            outputStream.Write(buffer, 0, buffer.Length);

            outputStream.Close(); //Peut-être que d'uniquement fermer le flux suffirait et permettrait de ne pas envoyer de data inutiles dans le flux pour rien.
        }

        static string GetJsonApi()
        {
            WebRequest request = WebRequest.Create("http://api.themoviedb.org/3/discover/movie?api_key=53583d53037bff6ba56435db8aca274e&certification_country=US&certification.lte=G&sort_by=popularity.desc&language=fr-FR&page=2");
            WebResponse rep = request.GetResponse();

            Stream repStream = rep.GetResponseStream();
            StreamReader sr = new StreamReader(repStream);
            string json = sr.ReadToEnd();
            sr.Close();
            return json;
        }



        static void Log(string text)
        {
            Console.WriteLine(text);
        }
    }
}


