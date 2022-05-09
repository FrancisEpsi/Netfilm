using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExoHttpAPI
{

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
            var bdd = new DatabaseInterface();
            var userLikeReq = "SELECT * FROM likes WHERE user_id='"+userID+"';";
            MySqlDataReader userLikeReader = bdd.SELECT(userLikeReq);
            var FilmIdsLiked = new List<int>();
            while (userLikeReader.Read())
            {
                FilmIdsLiked.Add(userLikeReader.GetInt32("film_id"));
            }

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


            //Console.WriteLine(JsonConvert.SerializeObject(this));
            return JsonConvert.SerializeObject(this);
        }

        public static string Get_TMDB_Discover_Json()
        {
            WebRequest request = WebRequest.Create("http://api.themoviedb.org/3/discover/movie?api_key=53583d53037bff6ba56435db8aca274e&certification_country=US&certification.lte=G&sort_by=popularity.desc&language=fr-FR&page=2");
            WebResponse rep = request.GetResponse();

            Stream repStream = rep.GetResponseStream();
            StreamReader sr = new StreamReader(repStream);
            string json = sr.ReadToEnd();
            sr.Close();
            return json;
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


