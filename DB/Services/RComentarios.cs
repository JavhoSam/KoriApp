using Firebase.Database;
using Firebase.Database.Query;
using KoriApp.DB.Models;
using Newtonsoft.Json;

namespace KoriApp.DB.Services
{
    class RComentarios
    {
        FirebaseClient Client = new FirebaseClient("");
        public async Task<bool> Save(Comentarios Comment)
        {
            var Data = await Client.Child(nameof(Comentarios)).PostAsync(JsonConvert.SerializeObject(Comment));
            if (!string.IsNullOrEmpty(Data.Key))
            {
                return true;
            }
            return false;
        }

        public async Task<List<Comentarios>> GetCommentsByPublishID(string publishID)
        {
            var comments = await Client.Child(nameof(Comentarios))
                                       .OrderBy("PublishID")
                                       .EqualTo(publishID)
                                       .OnceAsync<Comentarios>();

            return comments.Select(item => new Comentarios
            {
                ID = item.Key,
                PublishID = item.Object.PublishID,
                Comment = item.Object.Comment,
                UserPicURL = item.Object.UserPicURL,
                UserName = item.Object.UserName
            }).ToList();
        }
    }
}
