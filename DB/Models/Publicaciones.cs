using Newtonsoft.Json;

namespace KoriApp.DB.Models
{
    public class Publicaciones
    {
        public string ID { get; set; }
        public string PicURL { get; set; }
        public string Desc { get; set; }
        public string Hash { get; set; }
        public int Likes { get; set; }
        public int Comentarios { get; set; }
        public string Usuario { get; set; }
        public string UserPic { get; set; }
        public string CreatorID { get; set; }
        public string UiD { get; set; }
        public List<string> LikedBy { get; set; } = new List<string>();

        [JsonIgnore]
        public bool IsLikedByCurrentUser { get; set; }
    }
}
