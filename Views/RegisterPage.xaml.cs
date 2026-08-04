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

    // --- Métodos de visibilidad de contraseña ---
    private void OnTogglePassword_Clicked(object sender, EventArgs e)
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
    private void OnToggleConfirmPassword_Clicked(object sender, EventArgs e)
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
    private async void OnVolver_Clicked(object sender, EventArgs e)
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

    // --- Botón de Registro con UI de carga ---
    private async void OnRegistrar_Clicked(object sender, EventArgs e)
    {
        if (!ValidarCampos())
            return;

        // Activar indicador de carga
        btnRegistrar.Text = "";
        loadingIndicator.IsVisible = true;
        loadingIndicator.IsRunning = true;
        btnRegistrar.IsEnabled = false;

        try
        {
            var auth = CrossFirebaseAuth.Current;
            var authResult = await auth.CreateUserAsync(txtCorreo.Text, txtPassword.Text);

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

            var service = new UsuarioService();
            await service.GuardarUsuarioAsync(nuevoUsuario);

            SesionService.UsuarioActual = nuevoUsuario;

            await DisplayAlert("Éxito", "Cuenta creada correctamente", "OK");
            await Shell.Current.GoToAsync("//TourPage");
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