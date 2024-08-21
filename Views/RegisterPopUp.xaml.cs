using KoriApp.DB.Models;
using KoriApp.DB.Services;
using System.Text.RegularExpressions;

namespace KoriApp.Views;
public partial class RegisterPopUp
{
    Regex RegexPass = new Regex(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
    FirebaseConnection Conector;
    RUsuarios RUser;
    PicHelper PHelper = new PicHelper();
    private string PicURL;
    public byte[] ImageBytes { get; set; }
    public RegisterPopUp()
	{
		InitializeComponent();
	}
    private async void BtnRegistro_Clicked(object sender, EventArgs e)
    {
		LblStatus.TextColor = Colors.Red;
        LblStatus.IsEnabled = false;
        LblStatus.IsVisible = false;

        if (Validar())
		{
            BtnRegistro.Text = string.Empty;
            BtnRegistro.IsEnabled = false;
			Processor.IsRunning = true;

            try
			{
                var ImageStream = new MemoryStream(ImageBytes);
                PicURL = await PHelper.UploadProfilePicture(ImageStream, DateTime.Now.ToString());

                Usuarios Ruser = new Usuarios{
                   Email = TbxMail.Text,
                   UserName = TbxUser.Text,
                   UserDesc = "Añade una descripción... 🖊️",
                   UserPic = PicURL
                };

                RUser = new RUsuarios();
                await RUser.Save(Ruser);

                Conector = new FirebaseConnection();
				await FirebaseConnection.CreateUser(TbxMail.Text, TbxPass.Text);

                LblStatus.Text = "¡Usuario registrado exitosamente!";
				LblStatus.TextColor = Colors.ForestGreen;
                LblStatus.IsEnabled = true;
                LblStatus.IsVisible = true;
                Limpiar();
            }
			catch (Exception)
			{
                LblStatus.Text = "Ha ocurrido un error durante el registro";
                LblStatus.IsEnabled = true;
                LblStatus.IsVisible = true;
                return;
			}
			finally
			{
                BtnRegistro.Text = "Registrarte";
                Processor.IsRunning = false;
                BtnRegistro.IsEnabled = true;
            }
		}
    }
    private void Limpiar()
    {
        TbxMail.Text = string.Empty;
        TbxUser.Text = string.Empty;
        TbxPass.Text = string.Empty;
        ImgUserPic.Source = null;
    }

	private bool Validar()
	{

        if (string.IsNullOrEmpty(TbxUser.Text) || string.IsNullOrEmpty(TbxPass.Text) || string.IsNullOrEmpty(TbxUser.Text))
		{
            LblStatus.Text = "Debes rellenar todos los campos";
            LblStatus.IsEnabled = true;
            LblStatus.IsVisible = true;
            return false;
		}

        if (TbxPass.Text.Length < 8)
        {
            LblStatus.Text = "La contraseña debe contener al menos 8 carácteres";
            LblStatus.IsEnabled = true;
            LblStatus.IsVisible = true;
            return false;
        }

        if (TbxUser.Text.Length < 5)
        {
            LblStatus.Text = "El nombre de usuario es muy corto";
            LblStatus.IsEnabled = true;
            LblStatus.IsVisible = true;
            return false;
        }

        if (!RegexPass.IsMatch(TbxPass.Text))
		{
			LblStatus.Text = "La contraseña debe contener al menos: \n" +
				"* Una letra mayúscula \n" +
				"* Un número";
			LblStatus.IsEnabled = true;
            LblStatus.IsVisible = true;
            return false;
        }
		return true;
	}

    private async void BtnChoosePic_Clicked(object sender, EventArgs e)
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

            ImgUserPic.Source = ImageSource.FromStream(() => new MemoryStream(ImageBytes));
        }
    }
}