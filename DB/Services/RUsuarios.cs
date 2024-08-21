using Firebase.Database;
using Firebase.Database.Query;
using KoriApp.DB.Models;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace KoriApp.DB.Services
{
    public class RUsuarios
    {
        FirebaseClient Client = new FirebaseClient("");
        public async Task<bool> Save(Usuarios usuario)
        {
            var Data = await Client.Child(nameof(Usuarios)).PostAsync(JsonConvert.SerializeObject(usuario));
            if (!string.IsNullOrEmpty(Data.Key))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Update(Usuarios usuario)
        {
            var existingItem = await Client.Child(nameof(Usuarios)).Child(usuario.ID).OnceSingleAsync<Usuarios>();
            if (existingItem != null)
            {
                await Client.Child(nameof(Usuarios)).Child(usuario.ID).PutAsync(JsonConvert.SerializeObject(usuario));
                return true;
            }
            return false;
        }

        public async Task<string> GetUserNameByEmail(string email)
        {
            var usuarios = await Client.Child(nameof(Usuarios))
                                       .OrderBy("Email")
                                       .EqualTo(email)
                                       .OnceAsync<Usuarios>();

            foreach (var user in usuarios)
            {
                if (user.Object.Email == email)
                {
                    return user.Object.UserName;
                }
            }

            return null;
        }

        public async Task<string> GetUserPicByEmail(string email)
        {
            var usuarios = await Client.Child(nameof(Usuarios))
                                       .OrderBy("Email")
                                       .EqualTo(email)
                                       .OnceAsync<Usuarios>();

            foreach (var user in usuarios)
            {
                if (user.Object.Email == email)
                {
                    return user.Object.UserPic;
                }
            }

            return null;
        }

        public async Task<string> GetUserIdByEmail(string email)
        {
            var usuarios = await Client.Child(nameof(Usuarios))
                                       .OrderBy("Email")
                                       .EqualTo(email)
                                       .OnceAsync<Usuarios>();

            foreach (var user in usuarios)
            {
                if (user.Object.Email == email)
                {
                    return user.Key.ToString();
                }
            }

            return null;
        }

        public async Task<Usuarios> GetUserById(string userId)
        {
            var user = await Client.Child(nameof(Usuarios))
                                   .Child(userId)
                                   .OnceSingleAsync<Usuarios>();

            return user;
        }

    }
}
