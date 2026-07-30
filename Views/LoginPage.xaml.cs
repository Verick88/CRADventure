using CRadventure;
using CRadventure.Models;
using CRadventure.Services;
using CRADventure.Views;
using Plugin.Firebase;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;

namespace CRADventure.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    //Navega a la pagina de registro
    private async void Registrar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    // Toggle para mostrar/ocultar contraseña
    private void OnTogglePassword_Clicked(object sender, EventArgs e)
    {
        txtPassword.IsPassword = !txtPassword.IsPassword;
    }


    // Método principal de inicio de sesión con UI de carga
    private async void OnIniciarViaje_Clicked(object sender, EventArgs e)
    {
        // 1. Validar campos antes de proceder
        if (string.IsNullOrWhiteSpace(txtCorreo.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
            return;
        }

        // 2. Activar indicador de carga
        btnIniciar.Text = "";
        loadingIndicator.IsVisible = true;
        loadingIndicator.IsRunning = true;
        btnIniciar.IsEnabled = false;

        try
        {
            var auth = CrossFirebaseAuth.Current;
            var user = await auth.SignInWithEmailAndPasswordAsync(txtCorreo.Text, txtPassword.Text);

            // Obtener el rol del usuario desde Firestore
            var service = new UsuarioService();
            var usuario = await service.ObtenerUsuarioPorUidAsync(user.Uid);

            if (usuario == null)
            {
                await DisplayAlert("Error", "Usuario no encontrado en la base de datos", "OK");
            }
            else
            {
                // Navegar según el rol
                if (usuario.Rol == "admin" || usuario.Rol == "guia" || usuario.Rol == "cliente")
                    await Navigation.PushAsync(new TourPage(usuario));
                else
                    await DisplayAlert("Error", "Rol no reconocido", "OK");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error Login", "Correo o contraseña incorrectos", "OK");
        }
        finally
        {
            // 3. Restaurar botón al finalizar (éxito o error)
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
            btnIniciar.Text = "Iniciar viaje";
            btnIniciar.IsEnabled = true;
        }
    }
}