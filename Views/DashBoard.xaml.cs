using KoriApp.DB.Models;
using KoriApp.DB.Services;
using Mopups.Events;
using Mopups.Services;

namespace KoriApp.Views;
public partial class DashBoard : ContentPage
{
    public byte[] ImageBytes { get; set; }
    private Stream stream;
    PicHelper PHelper = new PicHelper();
    RPublicaciones RPubli = new RPublicaciones();
    FirebaseConnection Connection;
    MemoryStream ImageStream;
    private string PicURL;
    private string User;
    private string Pic;
    private string UserId;
    private string UID;
    private string UserMail;
    public DashBoard(string UserName, string UserPic, string UserID, string Uid, string Mail)
	{
		InitializeComponent();
        LblUserName.Text = UserName;
        ImgUserPic.Source = UserPic;
        User = UserName;
        Pic = UserPic;
        UserId = UserID;
        UID = Uid;
        UserMail = Mail;
        Obtener();
    }

    public async void Obtener()
    {
        var Publis = await RPubli.GetAll(UserId);
        ClvPublicaciones.ItemsSource = Publis;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
       MopupService.Instance.Popped += OnPopUpClosed;
       MopupService.Instance.PushAsync(new AddPublishPopUp(User, Pic, UserId, UID, ""), true);  
    }

    private void OnPopUpClosed(object? sender, PopupNavigationEventArgs e)
    {
        if (e.Page is AddPublishPopUp)
        {
            Obtener();
            MopupService.Instance.Popped -= OnPopUpClosed;
        }
    }

    private void BtnComment_Clicked(object sender, EventArgs e)
    {
        if (!(sender is ImageButton button)) return;

        string PublishID = button.CommandParameter.ToString();

        var Publish = ClvPublicaciones.ItemsSource.Cast<Publicaciones>().FirstOrDefault(a => a.ID == PublishID);

        if (Publish == null) return;

        Navigation.PushModalAsync(new Test(PublishID, User, Pic));
    }

    private async void BtnLogOut_Clicked(object sender, EventArgs e)
    {
        var Res = await DisplayAlert("Alerta", "¿Cerrar sesión?", "Confirmar", "Cancelar");

        if (Res)
        {
            await FirebaseConnection.SignOut();
            await Navigation.PopModalAsync();
        }
        else
        {
            return;
        }
    }

    private async void BtnLike_Clicked(object sender, EventArgs e)
    {
        if (!(sender is ImageButton button)) return;

        string PublishID = button.CommandParameter.ToString();

        var Publish = ClvPublicaciones.ItemsSource.Cast<Publicaciones>().FirstOrDefault(a => a.ID == PublishID);

        if (Publish == null) return;

        var result = await RPubli.ToggleLikePublication(PublishID, UserId);

        if (result)
        {
            Obtener();
        }
    }

    private void ImgUserPic_Clicked(object sender, EventArgs e)
    {
        if (!(sender is ImageButton button)) return;

        Navigation.PushModalAsync(new Profile(UserId, UserId, Pic, UID));
    }

    private void BtnPostProfile_Clicked(object sender, EventArgs e)
    {
        if (!(sender is ImageButton button)) return;

        string PostUserID = button.CommandParameter.ToString();

        var Publish = ClvPublicaciones.ItemsSource.Cast<Publicaciones>().FirstOrDefault(a => a.CreatorID == PostUserID);

        Navigation.PushModalAsync(new Profile(PostUserID, UserId, Publish.UserPic, UID));
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
                await RPubli.DeletePublication(PostID);
                Obtener();
            }
            else
            {
                return;
            }
        }

        if (Action == "Editar")
        {
            MopupService.Instance.Popped += OnPopUpClosed;
            await MopupService.Instance.PushAsync(new AddPublishPopUp(User, Pic, UserId, UID, PostID), true);
        }
    }
}

