namespace CRadventure;

using Plugin.Firebase.Auth;
using CRadventure.Models;
using CRadventure.Services;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    //Metodo para ver ocultar password
    private void Password_Clicked(object sender, EventArgs e)
    {
        txtPassword.IsPassword = !txtPassword.IsPassword;
        if (txtPassword.IsPassword)
        {
            btnOjoPassword.Source = "ocultar.png";
        }
        else
        {
            btnOjoPassword.Source = "ver.png";
        }
    }

    private void ConfirmPassword_Clicked(object sender, EventArgs e)
    {
        txtConfirmPassword.IsPassword = !txtConfirmPassword.IsPassword;
        if (txtConfirmPassword.IsPassword)
        {
            btnOjoConfirmPassword.Source = "ocultar.png";
        }
        else
        {
            btnOjoConfirmPassword.Source = "ver.png";
        }
    }
    //Boton regresar
    private async void Volver_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    //Validaciones
    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
            string.IsNullOrWhiteSpace(txtApellidos.Text) ||
            string.IsNullOrWhiteSpace(txtCorreo.Text) ||
            string.IsNullOrWhiteSpace(txtTelefono.Text) ||
            string.IsNullOrWhiteSpace(txtPassword.Text) ||
            string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
        {
            DisplayAlert("Error", "Por favor completa todos los campos", "OK");
            return false;
        }

        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
            return false;
        }

        if (txtPassword.Text.Length < 6)
        {
            DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres", "OK");
            return false;
        }

        return true;
    }

    // Boton de registro con carga
    private async void Registrar_Clicked(object sender, EventArgs e)
    {
        if (!ValidarCampos()) //Llama las validaciones, si falla algo cae en el return
            return;

        // Activar indicador de carga
        btnRegistrar.Text = "";
        loadingIndicator.IsVisible = true;
        loadingIndicator.IsRunning = true;
        btnRegistrar.IsEnabled = false;

        try
        {
            //Se crea un nuevo usuario con el servicio CreateUser
            var auth = CrossFirebaseAuth.Current;
            var authResult = await auth.CreateUserAsync(txtCorreo.Text, txtPassword.Text);

            //Arma el objeto del usuario
            var nuevoUsuario = new UsuarioModel
            {
                Uid = authResult.Uid,
                Nombre = txtNombre.Text,
                Apellidos = txtApellidos.Text,
                Email = txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Rol = "cliente",
                Activo = true,
                EsExtranjero = switchExtranjero.IsToggled,
                FechaRegistro = DateTime.UtcNow
            };

            //guarda los datos del usuario en el servicio
            var service = new UsuarioService();
            await service.GuardarUsuarioAsync(nuevoUsuario);

            SesionService.UsuarioActual = nuevoUsuario;

            await DisplayAlert("Éxito", "Cuenta creada correctamente", "OK");
            await Shell.Current.GoToAsync("//TourPage"); //Lo envia al tourpage
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo crear la cuenta: " + ex.Message, "OK");
        }
        finally
        {
            // Restaurar botón (se ejecuta ocurra error o éxito)
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
            btnRegistrar.Text = "Registrar e Iniciar";
            btnRegistrar.IsEnabled = true;
        }
    }
}