using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

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


