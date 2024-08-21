using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using KoriApp.DB.Models;
using Newtonsoft.Json;
using System.Web;

namespace KoriApp.DB.Services
{
    public class RPublicaciones
    {
        FirebaseClient Client = new FirebaseClient("");
        private FirebaseStorage Storage = new FirebaseStorage("");
        public async Task<bool> Save(Publicaciones publi)
        {
            var Data = await Client.Child(nameof(Publicaciones)).PostAsync(JsonConvert.SerializeObject(publi));
            if (!string.IsNullOrEmpty(Data.Key))
            {
                return true;
            }
            return false;
        }

        public async Task<List<Publicaciones>> GetAll(string currentUserId)
        {
            return (await Client.Child(nameof(Publicaciones)).OnceAsync<Publicaciones>()).Select(item => new Publicaciones
            {
                PicURL = item.Object.PicURL,
                Desc = item.Object.Desc,
                Likes = item.Object.Likes,
                Comentarios = item.Object.Comentarios,
                Usuario = item.Object.Usuario,
                Hash = item.Object.Hash,
                UserPic = item.Object.UserPic,
                CreatorID = item.Object.CreatorID,
                UiD = item.Object.UiD,
                LikedBy = item.Object.LikedBy ?? new List<string>(),
                IsLikedByCurrentUser = item.Object.LikedBy?.Contains(currentUserId) ?? false,
                ID = item.Key,
            }).ToList();
        }

        public async Task<Publicaciones> GetById(string id, string currentUserId)
        {
            var item = await Client.Child(nameof(Publicaciones)).Child(id).OnceSingleAsync<Publicaciones>();
            if (item != null)
            {
                return new Publicaciones
                {
                    ID = id,
                    PicURL = item.PicURL,
                    Desc = item.Desc,
                    Likes = item.Likes,
                    Comentarios = item.Comentarios,
                    Usuario = item.Usuario,
                    Hash = item.Hash,
                    UserPic = item.UserPic,
                    CreatorID = item.CreatorID,
                    UiD = item.UiD,
                    LikedBy = item.LikedBy ?? new List<string>(),
                    IsLikedByCurrentUser = item.LikedBy?.Contains(currentUserId) ?? false
                };
            }
            return null;
        }

        public async Task<bool> Update(Publicaciones publi)
        {
            var existingItem = await Client.Child(nameof(Publicaciones)).Child(publi.ID).OnceSingleAsync<Publicaciones>();
            if (existingItem != null)
            {
                await Client.Child(nameof(Publicaciones)).Child(publi.ID).PutAsync(JsonConvert.SerializeObject(publi));
                return true;
            }
            return false;
        }

        public async Task<bool> ToggleLikePublication(string publicationId, string userId)
        {
            // Obtén la publicación específica por ID
            var publicationSnapshot = await Client.Child(nameof(Publicaciones)).Child(publicationId).OnceSingleAsync<Publicaciones>();

            // Si la publicación no existe, devuelve false
            if (publicationSnapshot == null)
            {
                return false;
            }

            var publication = publicationSnapshot;

            // Verifica si el usuario ya ha dado Like
            if (publication.LikedBy.Contains(userId))
            {
                // Si ya ha dado Like, decrementa el contador y remueve al usuario de LikedBy
                publication.Likes--;
                publication.LikedBy.Remove(userId);
            }
            else
            {
                // Si no ha dado Like, incrementa el contador y añade al usuario a LikedBy
                publication.Likes++;
                publication.LikedBy.Add(userId);
            }

            // Actualiza la publicación en Firebase
            await Client.Child(nameof(Publicaciones)).Child(publicationId).PutAsync(JsonConvert.SerializeObject(publication));

            return true;
        }

        public async Task<bool> IncrementComments(string PostID)
        {
            try
            {
                var publication = (await Client.Child(nameof(Publicaciones)).Child(PostID).OnceSingleAsync<Publicaciones>());
                if (publication != null)
                {
                    publication.Comentarios++;
                    await Client.Child(nameof(Publicaciones)).Child(PostID).PutAsync(JsonConvert.SerializeObject(publication));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al incrementar comentarios: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Publicaciones>> GetUserPublications(string userId, string currentUserId)
        {
            var publicaciones = await Client.Child(nameof(Publicaciones)).OnceAsync<Publicaciones>();

            var userPublications = publicaciones
                .Where(p => p.Object.CreatorID == userId)
                .Select(p => new Publicaciones
                {
                    ID = p.Key,
                    UiD = p.Object.UiD,
                    PicURL = p.Object.PicURL,
                    Desc = p.Object.Desc,
                    Likes = p.Object.Likes,
                    Comentarios = p.Object.Comentarios,
                    CreatorID = p.Object.CreatorID,
                    Usuario = p.Object.Usuario,
                    Hash = p.Object.Hash,
                    UserPic = p.Object.UserPic,
                    LikedBy = p.Object.LikedBy ?? new List<string>(),
                    IsLikedByCurrentUser = p.Object.LikedBy?.Contains(currentUserId) ?? false,

                }).ToList();

            return userPublications;
        }

        public async Task<bool> DeletePublication(string publicationId)
        {
            try
            {
                await Client.Child(nameof(Publicaciones)).Child(publicationId).DeleteAsync();

                await DeleteCommentsByPublicationId(publicationId);

                var publication = (await Client.Child(nameof(Publicaciones)).Child(publicationId).OnceSingleAsync<Publicaciones>());
                if (publication != null && !string.IsNullOrEmpty(publication.PicURL))
                {
                    // Eliminar la imagen de Firebase Storage
                    await DeleteImageFromStorage(publication.PicURL);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task DeleteCommentsByPublicationId(string publicationId)
        {
            var comentarios = await Client.Child(nameof(Comentarios)).OrderBy("PublishID").EqualTo(publicationId).OnceAsync<Comentarios>();
            foreach (var comentario in comentarios)
            {
                await Client.Child(nameof(Comentarios)).Child(comentario.Key).DeleteAsync();
            }
        }

        public async Task DeleteImageFromStorage(string imageUrl)
        {
            try
            {
                // Extraer el nombre del archivo de la URL de la imagen
                var fileName = imageUrl.Split(new string[] { "%2F" }, StringSplitOptions.None)[1].Split('?')[0];

                // Eliminar el archivo de Firebase Storage
                await Storage.Child("publicaciones").Child(fileName).DeleteAsync();
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine($"Error al eliminar la imagen: {ex.Message}");
            }
        }
    }
}
