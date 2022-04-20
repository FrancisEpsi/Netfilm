using System;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Resources;

namespace ExoHttpAPI
{
    internal class Program
    {
        private static bool serverEnabled = false;
        private static int serverPort = 8081;
        static void Main(string[] args)
        {
            Log("Bienvenue sur le serveur d'API de film OpenMovie.");
            Log("Projet réalisé par François SAURA et Loïc LABAISSE");
            Log("");
            serverEnabled = true;
            ReadRequest();
        }

        static void ReadRequest()
        {
            var listenner = new HttpListener();
            listenner.Prefixes.Add("http://*:" + serverPort + "/");
            
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
                Log("Requête reçu par " + conn.Request.RemoteEndPoint.ToString() + "(" + conn.Request.RawUrl + ")");
                ExecuteRequest(conn);
            }

            listenner.Stop();

        }

        static void ExecuteRequest(HttpListenerContext conn)
        {
            string Route = conn.Request.RawUrl;

            if (conn.Request.RawUrl == "/")
            {
                SendHtmlResponse(conn, "<html><body><h1>Projet Site de films</h1><h2><u>Matiere:</u> Service WEB: Communication et echange de donnees</h2><h2><u>Participants:</u> Francois SAURA et Loic LABAISSE</h2><h2><u>Objectifs:</u> Developper un site internet d informations cinematographique en utilisant l API de TheMovieDB</h2><h1 style='text-align: center;'>BIENVENUE SUR LE SERVEUR API (Backend)</h1><p style='text-align: center;'>Veuillez utiliser convenablement l API avec une URI valide.</p></body></html>");
                return;
            }

            string[] Path = Route.Split("/");
            if (Path[1] == "Films")
            {
                string jsonString = GetJsonApi();
                SendJsonResponse(conn, jsonString);
            } else if (Path[1] == "Login")
            {
                string Username = Path[2];
                SendHtmlResponse(conn, "Login de l'utilisateur : " + Username);
            } else
            {
                SendHtmlResponse(conn, Route + "<br>la route spécifié n'est pas définie.");
            }


        }

        static void SendHtmlResponse(HttpListenerContext conn, string html)
        {
            HttpListenerResponse rep = conn.Response;
            rep.ContentType = "text/html; charset=utf-8";
            rep.ContentEncoding = System.Text.Encoding.UTF8;
            byte[] htmlBytes = System.Text.Encoding.UTF8.GetBytes(html);
            rep.ContentLength64 = htmlBytes.Length;
            Stream outputStream = rep.OutputStream;
            outputStream.Write(htmlBytes, 0, htmlBytes.Length);
            outputStream.Close();
        }

        static void SendJsonResponse(HttpListenerContext conn, string jsonString)
        {
            HttpListenerResponse rep = conn.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonString); //Conversion du json qui va être retourné en binaire

            //Ajout des métadonnées de la réponse HTTP (headers):
            rep.ContentLength64 = buffer.Length;
            rep.ContentType = "application/json";
            rep.ContentEncoding = System.Text.Encoding.UTF8;
            rep.AppendHeader("Access-Control-Allow-Origin", "*");

            Stream outputStream = rep.OutputStream;
            outputStream.Write(buffer, 0, buffer.Length);
            outputStream.Close(); //Fermer le flux ici ?
        }

        

        static String getJson()
        {
            Personnage p1 = new Personnage();
            p1.prenom = "François";
            p1.nom = "SAURA";
            p1.age = 21;

            var json = JsonConvert.SerializeObject(p1);
            return json;
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


