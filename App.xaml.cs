using KoriApp.Views;

namespace KoriApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Login();
        }
    }
}
