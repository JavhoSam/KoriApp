using KoriApp.DB.Models;
using KoriApp.DB.Services;
using Mopups.Services;

namespace KoriApp.Views;
public partial class AddPublishPopUp
{
    public byte[] ImageBytes { get; set; }
    PicHelper PHelper = new PicHelper();
    RPublicaciones RPubli = new RPublicaciones();
    Publicaciones Publi;
    private string PicURL;
    private string User;
    private string UserPicURL;
    private string Userid;
    private string Uid;
    private string PID;
    private bool EditFlag;
    public AddPublishPopUp(string UserName, string UserPic, string UserID, string UID, string PublishID)
	{
		InitializeComponent();
        User = UserName;
        UserPicURL = UserPic;
        Userid = UserID;
        Uid = UID;
        PID = PublishID;
        if (PID != "")
        {
            LblTitle.Text = "Editar publicación";
            BtnAddPic.IsVisible = false;
            GetEditData();
        }
        else
        {
            return;
        }
	}

    private async void GetEditData()
    {
        var Data = await RPubli.GetById(PID, Userid);

        TbxDesc.Text = Data.Desc;
        TbxHash.Text = Data.Hash;
        ImgFoto.Source = Data.PicURL;

        Publi = new Publicaciones
        {
            ID = PID,
            PicURL = Data.PicURL,
            Desc = Data.Desc,
            Likes = Data.Likes,
            Comentarios = Data.Comentarios,
            Usuario = Data.Usuario,
            Hash = Data.Hash,
            UserPic = Data.UserPic,
            CreatorID = Data.CreatorID,
            UiD = Data.UiD,
            LikedBy = Data.LikedBy,
            IsLikedByCurrentUser = Data.IsLikedByCurrentUser
        };

        EditFlag = true;
    }

    private async void BtnAddPic_Clicked(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Selecciona una imagen"
        });

        if (result != null)
        {
            var stream = await result.OpenReadAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            ImageBytes = memoryStream.ToArray();
            var ImageStream = new MemoryStream(ImageBytes);

            ImgFoto.Source = ImageSource.FromStream(() => new MemoryStream(ImageBytes));
        }
    }

    private async void BtnSavePic_Clicked(object sender, EventArgs e)
    {
        if (ImgFoto.Source == null)
        {
            await DisplayAlert("Alerta", "Agrega una foto a la publicación", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(TbxDesc.Text))
        {
            await DisplayAlert("Alerta", "Ingresa una descripción", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(TbxHash.Text))
        {
            await DisplayAlert("Alerta", "Ingresa un HashTag", "Ok");
            return;
        }

        Process.IsRunning = true;
        BtnSavePic.IsEnabled = false;
        BtnSavePic.Text = string.Empty;

        if (EditFlag)
        {
            try
            {
                Publi.ID = PID;
                Publi.Desc = TbxDesc.Text;
                Publi.Hash = TbxHash.Text;

                var Res = await RPubli.Update(Publi);
                if (Res)
                {
                    await DisplayAlert("Alerta", "Publicación actualizada correctamente", "Oskers");
                    await MopupService.Instance.PopAsync();
                    return;
                }
                else
                {
                    await DisplayAlert("Alerta", "Ha ocurrido un erro al actulizar la publicación", "Oskers");

                    return;
                }
            }
            catch (Exception Ex)
            {
                await DisplayAlert("Alerta", "Ha ocurrido un erro al actulizar la publicación", "Oskers");
                return;
            }
            finally {
                BtnSavePic.IsEnabled = true;
                Process.IsRunning = false;
                BtnSavePic.Text = "Agregar";
            }
        }
        else
        {
            try
            {
                var ImageStream = new MemoryStream(ImageBytes);
                PicURL = await PHelper.UploadPostPic(ImageStream, DateTime.Now.ToString());

                Publicaciones publicaciones = new Publicaciones
                {
                    Desc = TbxDesc.Text,
                    Likes = 0,
                    Comentarios = 0,
                    PicURL = this.PicURL,
                    Hash = TbxHash.Text,
                    UserPic = UserPicURL,
                    CreatorID = Userid,
                    UiD = Uid,
                    Usuario = User
                };

                RPubli = new RPublicaciones();
                var Data = await RPubli.Save(publicaciones);

                await DisplayAlert("Alerta", "Publicación guardada exitosamente", "Ok");
                PHelper = new PicHelper();

                await MopupService.Instance.PopAsync();
            }
            catch (Exception Ex)
            {
                await DisplayAlert("Alerta", "Ha ocurrido un error al crear la publicación" + Ex.Message, "Ok");
                return;
            }
            finally
            {
                BtnSavePic.IsEnabled = true;
                Process.IsRunning = false;
                BtnSavePic.Text = "Agregar";
            }
        }
    }
}