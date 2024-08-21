using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace KoriApp.DB.Models
{
    public class Comentarios
    {
        public string ID { get; set; }
        public string PublishID { get; set; }
        public string UserPicURL { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; }
    }
}
