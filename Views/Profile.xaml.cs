using Firebase.Auth;
using KoriApp.DB.Models;
using KoriApp.DB.Services;
using Mopups.Events;
using Mopups.Services;

namespace KoriApp.Views;
public partial class Profile : ContentPage
{
	RUsuarios RUser = new RUsuarios();
	RPublicaciones RPosts = new RPublicaciones();
    Usuarios Usuario;

	private string User;
    private string CurrentID;
    private string UserPic;
    private string Uid;
	public Profile(string UserID, string CurrentUserID, string UserPicUrl, string UID)
	{
		InitializeComponent();
		User = UserID;
        CurrentID = CurrentUserID;
        UserPic = UserPicUrl;
        Uid = UID;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Yield();
        FetchUserData();
    }

    private async void FetchUserData()
	{
		var UserInfo = await RUser.GetUserById(User);
		BtnUserPic.Source = UserInfo.UserPic;
		LblUserMail.Text = UserInfo.Email;
		LblUserName.Text = UserInfo.UserName;
		LblUserDesc.Text = UserInfo.UserDesc;
		LblSub.Text = "Publicaciones de " + UserInfo.UserName;
		GetUserPosts();
	}

	private async void GetUserPosts()
	{
        var Publicaciones = await RPosts.GetUserPublications(User, CurrentID);
		ClvPublicaciones.ItemsSource = Publicaciones;
    }

    private async void BtnLike_Clicked(object sender, EventArgs e)
    {
        if (!(sender is ImageButton button)) return;

        string PublishID = button.CommandParameter.ToString();

        var Publish = ClvPublicaciones.ItemsSource.Cast<Publicaciones>().FirstOrDefault(a => a.ID == PublishID);

        if (Publish == null) return;

        var result = await RPosts.ToggleLikePublication(PublishID, User);

        if (result)
        {
            GetUserPosts();
        }
    }

    private void BtnComment_Clicked(object sender, EventArgs e)
    {
        if (!(sender is ImageButton button)) return;

        string PublishID = button.CommandParameter.ToString();

        var Publish = ClvPublicaciones.ItemsSource.Cast<Publicaciones>().FirstOrDefault(a => a.ID == PublishID);

        if (Publish == null) return;

        Navigation.PushModalAsync(new Test(PublishID, LblUserName.Text, UserPic));
    }

    private async void BtnPostOptions_Clicked(object sender, EventArgs e)
    {
        if (!(sender is Button button)) return;

        string PostID = button.CommandParameter.ToString();

        string Action = await DisplayActionSheet("Opciones", "Cancelar", null, "Borrar", "Editar");

        if (Action == "Borrar")
        {
            bool Res = await DisplayAlert("Alerta", "¿Quieres borrar la publicación?", "Confirmar", "Cancelar");
            if (Res)
            {
                await RPosts.DeletePublication(PostID);
                FetchUserData();
            }
            else
            {
                return;
            }
        }

        if (Action == "Editar")
        {
            MopupService.Instance.Popped += OnPopUpClosed;

            var Post = await RPosts.GetById(PostID, CurrentID);
            await MopupService.Instance.PushAsync(new AddPublishPopUp(LblUserName.Text, Post.UserPic, User, Uid, PostID), true);
        }
    }

    private void OnPopUpClosed(object? sender, PopupNavigationEventArgs e)
    {
        if (e.Page is AddPublishPopUp)
        {
            FetchUserData();
            MopupService.Instance.Popped -= OnPopUpClosed;
        }
    }
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (CurrentID != User) return;

        var Descripcion = await DisplayPromptAsync("Estado", "Ingresa una nueva biografía a continuación:");
        if (Descripcion == null) return;

        Usuario = new Usuarios
        {
            ID = User,
            UserDesc = Descripcion,
            Email = LblUserMail.Text,
            UserName = LblUserName.Text,
            UserPic = this.UserPic
        };

        var Res = await RUser.Update(Usuario);
        if (Res)
        {
            FetchUserData();
            return;
        }
        else
        {
            await DisplayAlert("Alerta", "Ha ocurrido un error al actualizar la biografía", "Ok");
            return;
        }
    }
}