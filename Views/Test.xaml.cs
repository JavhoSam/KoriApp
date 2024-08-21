using KoriApp.DB.Models;
using KoriApp.DB.Services;

namespace KoriApp.Views;
public partial class Test : ContentPage
{
    RComentarios RComm = new RComentarios();
    RPublicaciones RPub = new RPublicaciones();
    private string Publish_ID;
    private string User_Name;
    private string UserPic;

    public Test(string ID, string UserName, string UserpicURL)
	{
		InitializeComponent();
        Publish_ID = ID;
        User_Name = UserName;
        UserPic = UserpicURL;
        Obtener();
	}

	private async void Obtener()
	{
        var Coments = await RComm.GetCommentsByPublishID(Publish_ID);
        ClvComentarios.ItemsSource = Coments;
    }

    private async void BtnComment_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TbxComentario.Text))
        {
            return;
        }
        else
        {
            try
            {
                Comentarios Com = new Comentarios
                {
                    UserPicURL = UserPic,
                    UserName = User_Name,
                    Comment = TbxComentario.Text,
                    PublishID = Publish_ID
                };

                var Res = await RComm.Save(Com);
                await RPub.IncrementComments(Publish_ID);

                if (Res)
                {
                    TbxComentario.Text = string.Empty;
                    Obtener();
                }
                else
                {
                    await DisplayAlert("Alerta", "Ha ocurrido un error al guardar el comentario", "Ok");
                    return;
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Alerta", "Ha ocurrido un error al guardar el comentario", "Ok");
                return;
            }
        }
    }

    private void BtnBack_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}