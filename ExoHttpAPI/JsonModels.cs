using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExoHttpAPI
{
    //Ce fichier contient plusieurs classes permettant l'échange de donnée avec le FrontEnd.

    /// <summary>
    /// Classe permettant d'intégrer les données d'une réponse à une requête HTTP GET de connexion d'un utilisateur.
    /// </summary>
    public class LoginResponse
    {

        public int id;
        public string first_name;
        public string last_name;
        public string access_token;
        public bool exist = false;
        public string ErrorComment;

        public LoginResponse() {}

        /// <summary>
        /// Intérroge la base de donnée et complète les propriétées de la classe si les identifiants de connexion fournies sont correct. S'ils sont incorrect, la propriété exist sera définit à FALSE et les autres propriétées seront nulles.
        /// </summary>
        /// <param name="email">L'adresse e-mail qu'a saisi l'utilisateur sur le FrontEnd</param>
        /// <param name="passhash">Le mot de passe qu'a saisit l'utilisateur et hashé par le FrontEnd</param>
        public LoginResponse(string email, string passhash)
        {
            var bdd = new DatabaseInterface();
            MySqlDataReader reader = bdd.SELECT("SELECT * FROM users WHERE email='" + email + "' AND password = '" + passhash + "';");
            if (reader == null) { this.ErrorComment = "Connexion à la base de donnée impossible"; return; }
            if (reader.HasRows == false) { this.ErrorComment = "Email ou mot de passe incorrect";  return; }
            while (reader.Read())
            {
                this.id = reader.GetInt32("id");
                this.first_name = reader.GetString("prenom");
                this.last_name = reader.GetString("nom");
                exist = true;
                //Access token à générer et à définir ici
            }
            reader.Close();
        }

        /// <summary>
        /// Sérialise l'objet actuel avec un formattage JSON
        /// </summary>
        /// <returns>Un string contenant les propriétées de l'objet sérialisé au format JSON (prêt à l'envoi). Retourne null si le sérialisation à échoué.</returns>
        public string getJsonResponse()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            }
            catch
            {
                return null;
            }
        }

    }

    /// <summary>
    /// Classe permettant de définir les données d'un jaquette de film dans le contexte d'un utilisateur
    /// </summary>
    public class Film
    {
        public int id;
        public string titre;
        public string description;
        public string synopsis;
        public string image_url;
        public bool user_liked;
        public bool user_loved;
        public bool moviedb_json;
    }

    public class DiscoverResponse
    {
        public List<Film> films = new List<Film>();

        public string Make(int userID = 10)
        {
            //On récupère la liste des ID de film que l'utilisateur à aimer:
            Console.WriteLine("Récupération de la liste des likes de userID = " + userID + "...");
            var t1 = DateTime.Now;
            var bdd = new DatabaseInterface();
            var userLikeReq = "SELECT * FROM likes WHERE user_id='"+userID+"';";
            MySqlDataReader userLikeReader = bdd.SELECT(userLikeReq);
            var FilmIdsLiked = new List<int>();
            while (userLikeReader.Read())
            {
                FilmIdsLiked.Add(userLikeReader.GetInt32("film_id"));
            }
            var TimeElapsed1 = DateTime.Now - t1;
            Console.WriteLine(TimeElapsed1.TotalMilliseconds + " ms se sont déroulée pour récupérer la liste des likes de userID = " + userID);
            Console.WriteLine("");
            Console.WriteLine("Récupération de la page Discover aurpès de TheMovieDB...");
            var t2 = DateTime.Now;
            //On fait l'appel à l'API de The Movie Database pour obtenir la page discover (pleins de films)
            string TMDB_Json = Get_TMDB_Discover_Json();
            if (TMDB_Json == null) { return null; }

            JObject TMDB_Obj = JObject.Parse(TMDB_Json);
            JToken TMDB_Films = TMDB_Obj["results"];
            foreach (JToken TMDB_Film in TMDB_Films)
            {
                //Pour chaque film dans la réponse de l'API de TheMovieDB on créer notre propre objet film ou on y complète ses propriétées:
                Film film = new Film();
                film.id = (int)TMDB_Film["id"];
                film.titre = (string)TMDB_Film["title"];
                film.image_url = (string)TMDB_Film["poster_path"];
                film.synopsis = (string)TMDB_Film["overview"];
                if (FilmIdsLiked.Contains(film.id)) { film.user_liked = true; } else { film.user_liked = false; }
                //Puis on ajoute ce nouvel objet film à notre liste de film:
                this.films.Add(film);
            }
            var TimeElapsed2 = DateTime.Now - t2;
            var TotalTimeElapsed = DateTime.Now - t1;
            Console.WriteLine(TimeElapsed2.TotalMilliseconds + " ms se sont écoulées pour requêtter l'API de TheMovieDB");
            Console.WriteLine(TotalTimeElapsed.TotalMilliseconds + " ms se sont déroulées au total pour générer la liste des films avec les likes attribués ou pas à ces films pour l'userID = " + userID);


            //Console.WriteLine(JsonConvert.SerializeObject(this));
            return JsonConvert.SerializeObject(this);
        }

        public bool MakeFromExistingApiCall(string TMDB_Discover_Json, int userID)
        {
            var bdd = new DatabaseInterface();
            var userLikeReq = "SELECT * FROM likes WHERE user_id='" + userID + "';";
            MySqlDataReader userLikeReader = bdd.SELECT(userLikeReq);
            var FilmIdsLiked = new List<int>();
            if (userLikeReader == null)
            {
                return false;
            }
            while (userLikeReader.Read())
            {
                FilmIdsLiked.Add(userLikeReader.GetInt32("film_id"));
            }

            if (TMDB_Discover_Json == null) { return false; }

            JObject TMDB_Obj = JObject.Parse(TMDB_Discover_Json);
            JToken TMDB_Films = TMDB_Obj["results"];
            foreach (JToken TMDB_Film in TMDB_Films)
            {
                //Pour chaque film dans la réponse de l'API de TheMovieDB on créer notre propre objet film ou on y complète ses propriétées:
                Film film = new Film();
                film.id = (int)TMDB_Film["id"];
                film.titre = (string)TMDB_Film["title"];
                film.image_url = (string)TMDB_Film["poster_path"];
                film.synopsis = (string)TMDB_Film["overview"];
                if (FilmIdsLiked.Contains(film.id)) { film.user_liked = true; } else { film.user_liked = false; }
                //Puis on ajoute ce nouvel objet film à notre liste de film:
                this.films.Add(film);
            }
            return true;
        }

        public string getJsonString()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            } catch
            {
                return null;
            }
        }

        public static string Get_TMDB_Discover_Json()
        {
            WebRequest request = WebRequest.Create("http://api.themoviedb.org/3/discover/movie?api_key=" + Program.TMDB_API_KEY + "&certification_country=US&certification.lte=G&sort_by=popularity.desc&language=fr-FR&page=2");
            WebResponse rep = request.GetResponse();

            Stream repStream = rep.GetResponseStream();
            StreamReader sr = new StreamReader(repStream);
            string json = sr.ReadToEnd();
            sr.Close();
            return json;
        }
    }

    public class PutLikeResponse
    {
        public int user_id = 0;
        public int id_film = 0;
        public bool user_liked;

        public bool LoadRequest(string body)
        {
            Console.WriteLine("LoadRequest(" + body + ")");
            JObject reqObj = null;
            try
            {
                reqObj = JObject.Parse(body);
            } catch (Exception ex)
            {
                Console.WriteLine("PutLikeResponse.LoadRequest() : Error parsing JSON. Détail: " + ex.Message);
                return false;
            }
            if (reqObj == null) { Console.WriteLine("LoadRequest() : L'objet JSON est vide !"); return false; }

            try
            {
                if (reqObj.ContainsKey("user_id")) { this.user_id = Convert.ToInt32(reqObj.GetValue("user_id").ToString()); } else { Console.WriteLine("LoadRequest() : La clé user_id dans le JSON n'existe pas !");  return false; }
                if (reqObj.ContainsKey("id_film")) { this.id_film = Convert.ToInt32(reqObj.GetValue("id_film").ToString()); } else { Console.WriteLine("LoadRequest() : La clé id_film dans le JSON n'existe pas !"); return false; }
                if (reqObj.ContainsKey("user_liked")) { this.user_liked = reqObj.GetValue("user_liked").ToObject<bool>(); } else { Console.WriteLine("LoadRequest() : La clé user_liked dans le JSON n'existe pas !"); return false; }
            } catch
            {
                Console.WriteLine("PutLikeResponse.LoadRequest() : Error reading and casting JSON Keys to object propertie's");
                Console.WriteLine("Voici ce qui à été envoyé du client:");
                Console.WriteLine(body);
                return false;
            }

            //Console.WriteLine("Demande PUTLIKE pour user_id = '" + userID + "' et id_film = '" + filmID + "' et user_liked = '" + setLike + "' en cours de traitement...");
            return true;
        }

        public bool Execute()
        {
            var bdd = new DatabaseInterface();
            MySqlDataReader userLikeReader;

            if (user_liked == true)
            {
                string userLikeRequest = "SELECT id FROM likes WHERE user_id='" + user_id + "' AND film_id='" + id_film + "';";
                userLikeReader = bdd.SELECT(userLikeRequest);
                if (userLikeReader == null) { Console.WriteLine("PutLikeResponse.Execute() : Retourne FALSE car le lecteur de la requête SQL permettant de vérifier si le like existe déjà n'a pas pu aboutir à sa vérification."); return false; }
                if (userLikeReader.HasRows) { return true; }
                //Si la mention j'aime n'existe pas:
                string likeRequest = "INSERT INTO likes (user_id, film_id) VALUES ('"+user_id+"', '"+id_film+"');";
                return bdd.EXECUTE_REQUEST(likeRequest);
            } else if (user_liked == false)
            {
                string deleteLikeRequest = "DELETE FROM likes WHERE user_id='" + this.user_id + "' AND film_id='" + this.id_film + "';";
                return bdd.EXECUTE_REQUEST(deleteLikeRequest);
            } else
            {
                Console.WriteLine("PutLikeResponse.Execute() : Retourne FALSE car l'opération à effectuer n'est pas défini (PutLikeResponse.user_liked n'est n'y TRUE n'y FALSE)");
                return false;
            }

        }

        public void ReverseLike()
        {
            if (this.user_liked == true) { this.user_liked = false; } else if (this.user_liked == false) { this.user_liked = true; }
        }

        public string getJsonReponse()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class UserLikesResponse
    {
        public int user_id;
        string TMDB_BaseMoviesList;
        public List<Film> MoviesLiked = new List<Film>();
        public int statusCode = 200;
        public string errorComment = null;

        public bool Load(string jsonString, string TMDB_BaseMoviesList)
        {
            this.TMDB_BaseMoviesList = TMDB_BaseMoviesList;
            return true;
        }

        public bool GetLikes()
        {
            var bdd = new DatabaseInterface();
            string userLikeRequest = "SELECT id FROM likes WHERE user_id='" + user_id + "';";
            MySqlDataReader userLikeReader = bdd.SELECT(userLikeRequest);
            if (userLikeReader == null) { statusCode = 500; errorComment = "Requête SQL surement invalide"; Console.WriteLine("UserLikesResponse.GetLikes() : Retourne FALSE car le lecteur de la requête SQL permettant de lire les ID des films que l'utilisateur à aimer n'a pas pu aboutir"); return false; }
            if (userLikeReader.HasRows) { return true; }

            while (userLikeReader.Read())
            {
                int movie_id = userLikeReader.GetInt32("id");
                Film film = null;
                film = getFilmByID(movie_id);
                if (film != null)
                {
                    film.user_liked = true;
                    this.MoviesLiked.Add(film);
                }
            }

            return true;
        }

        public Film getFilmByID(int id_film)
        {
            JObject TMDB_Obj = JObject.Parse(TMDB_BaseMoviesList);
            JToken TMDB_Films = TMDB_Obj["results"];
            foreach (JToken TMDB_Film in TMDB_Films)
            {
                Film film = new Film();
                film.id = (int)TMDB_Film["id"];
                film.titre = (string)TMDB_Film["title"];
                film.image_url = (string)TMDB_Film["poster_path"];
                film.synopsis = (string)TMDB_Film["overview"];
                if (film.id == id_film)
                {
                    return film;
                }
            }
            return null;
        }

        public string getJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }



    /// <summary>
    /// Classe permettant de définir des données à retourner lors d'une réponse à une requête HTTP GET
    /// </summary>
    public class GetResponse
    {
        /// <summary>
        /// Le status HTML de la réponse à envoyer (se référer à l'énumération StatusHTML)
        /// </summary>
        public int statusCode = 200;
        /// <summary>
        /// Un commentaire pour expliquer l'action ou l'erreur qui vient de se produire
        /// </summary>
        public string comment;

        /// <summary>
        /// Sérialise l'objet actuel avec un formattage JSON
        /// </summary>
        /// <returns>Un string contenant les propriétées de l'objet sérialisé au format JSON (prêt à l'envoi). Retourne null si le sérialisation à échoué.</returns>
        public string getJsonResponse()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            } catch
            {
                return null;
            }
        }
    }

    public enum StatusHTML
    {
        OK = 200,
        ERROR = 400,
    }
}


