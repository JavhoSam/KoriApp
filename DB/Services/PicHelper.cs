using Firebase.Auth.Providers;
using Firebase.Storage;

namespace KoriApp.DB.Services
{
    public class PicHelper
    {
        public async Task<string> UploadProfilePicture(Stream archivo, string nombre)
        {
            string ruta = ";

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                ruta,
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                })
                .Child("Fotos_Perfil")
                .Child(nombre)
                .PutAsync(archivo, cancellation.Token);

            var downloadURL = await task;

            return downloadURL;
        }

        public async Task<string> UploadPostPic(Stream archivo, string nombre)
        {
            string ruta = "";

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                ruta,
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                })
                .Child("Fotos_Posts")
                .Child(nombre)
                .PutAsync(archivo, cancellation.Token);

            var downloadURL = await task;

            return downloadURL;
        }


    }
}
