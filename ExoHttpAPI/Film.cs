using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoHttpAPI
{
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
}
