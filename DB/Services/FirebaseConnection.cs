using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;

namespace KoriApp.DB.Services
{
    internal class FirebaseConnection
    {
        private TaskCompletionSource<string>? redirectCompletionSource;
        public static FirebaseAuthClient ConectingFirebase()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "",
                AuthDomain = "",
                Providers = new Firebase.Auth.Providers.FirebaseAuthProvider[]
                {
                    new GoogleProvider().AddScopes("email"),
                    new EmailProvider()
                },
                UserRepository = new FileUserRepository("SmartCard")
            };
            var client = new FirebaseAuthClient(config);
            return client;
        }

        public static async Task<UserCredential> CreateUser(string Email, string Password)
        {
            var cliente = ConectingFirebase();
            var userCredential = await cliente.CreateUserWithEmailAndPasswordAsync(Email, Password);
            return userCredential;
        }
        public static async Task<UserCredential> LoadUser(string Email, string Password)
        {
            var cliente = ConectingFirebase();

            var userCredential = await cliente.SignInWithEmailAndPasswordAsync(Email, Password);
            return userCredential;
        }

        public static async Task SignOut()
        {
            var client = ConectingFirebase();
            client.SignOut();
        }

        public static FirebaseAuthClient GetCurrentUser()
        {
            var cliente = ConectingFirebase();
            return cliente;
        }
    }
}
