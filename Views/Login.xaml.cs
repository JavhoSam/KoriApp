using Firebase.Auth;
using KoriApp.DB.Services;
using Mopups.Services;

namespace KoriApp.Views;

public partial class Login : ContentPage
{
    DashBoard Main;
    RUsuarios RUsers;
	public Login()
	{
		InitializeComponent();
        RUsers = new RUsuarios();
	}

    private void BtnRegister_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PushAsync(new RegisterPopUp(), true);
    }

    private void Limpiar()
    {
        TbxMail.Text = string.Empty;
        TbxPass.Text = string.Empty;
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TbxMail.Text) || string.IsNullOrEmpty(TbxPass.Text))
        {
            await DisplayAlert("Alerta", "Ingresa tus datos para iniciar sesión", "Ok");
            return;
        }

        BtnLogin.IsEnabled = false;
        BtnLogin.Text = string.Empty;
        Process.IsRunning = true;
        TbxMail.IsEnabled = false;
        TbxPass.IsEnabled = false;

        try
        {       
            var UserName = await RUsers.GetUserNameByEmail(TbxMail.Text.Trim());
            var UserPic = await RUsers.GetUserPicByEmail(TbxMail.Text.Trim());
            var UserID = await RUsers.GetUserIdByEmail(TbxMail.Text.Trim());

            if (UserName == null || UserPic == null || UserID == null)
            {
                LblStatus.Text = "Ha ocurrido un error al recuperar la información del usuario";
                LblStatus.IsEnabled = true;
                LblStatus.IsVisible = true;
                return;
            }

            var Data = await FirebaseConnection.LoadUser(TbxMail.Text.Trim(), TbxPass.Text.Trim());

            await Navigation.PushModalAsync(new DashBoard(UserName, UserPic, UserID, Data.User.Uid, TbxMail.Text));
            Limpiar();
        }
        catch(FirebaseAuthException Ex){
            LblStatus.Text = "Correo y/o contraseña incorrectos";
            LblStatus.IsEnabled = true;
            LblStatus.IsVisible = true;
            return;
        }
        catch (Exception Ex)
        {
            LblStatus.Text = "Ha ocurrido un error, intenta de nuevo más tarde.";
            LblStatus.IsEnabled = true;
            LblStatus.IsVisible = true;
            return;
        }
        finally
        {
            BtnLogin.IsEnabled = true;
            BtnLogin.Text = "Ingresar";
            Process.IsRunning = false;
            TbxMail.IsEnabled = true;
            TbxPass.IsEnabled = true;
        }
    }

    private void TbxPass_Completed(object sender, EventArgs e)
    {
        BtnLogin_Clicked(sender, e);
    }
}