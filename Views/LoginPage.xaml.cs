using CRadventure;
using CRadventure.Models;
using CRadventure.Services;
using CRadventure.Views;
using Plugin.Firebase;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;

namespace CRadventure.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    // Navega a la pagina de registro
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
            var user = await auth.SignInWithEmailAndPasswordAsync(txtCorreo.Text.Trim(), txtPassword.Text);

            if (user == null)
            {
                await DisplayAlert("Error", "No se pudo autenticar el usuario", "OK");
                return;
            }

            // Obtener el rol del usuario desde Firestore
            var service = new UsuarioService();
            var usuario = await service.ObtenerUsuarioPorUidAsync(user.Uid);

            if (usuario == null)
            {
                // Si el usuario existe en Auth pero no en Firestore, lo creamos automáticamente
                usuario = new UsuarioModel
                {
                    Uid = user.Uid,
                    Email = user.Email ?? txtCorreo.Text.Trim(),
                    Nombre = "Usuario",
                    Apellidos = "",
                    Telefono = "",
                    Rol = "cliente",
                    Activo = true,
                    FechaRegistro = DateTimeOffset.Now
                };

                // Guardar el nuevo documento en Firestore usando su UID como ID de documento
                await CrossFirebaseFirestore.Current
                    .GetCollection("usuarios")
                    .GetDocument(user.Uid)
                    .SetDataAsync(usuario);
            }

            // Se guarda el usuario con el servicio de sesion
            SesionService.UsuarioActual = usuario;

            // Navegar según el rol
            if (usuario.Rol == "admin" || usuario.Rol == "guia" || usuario.Rol == "cliente")
                await Shell.Current.GoToAsync("//TourPage");
            else
                await DisplayAlert("Error", "Rol no reconocido", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error Técnico de Firebase", ex.Message, "OK");
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

    // Método para recuperar contraseña
    private async void OnOlvidastePasswordTapped(object sender, EventArgs e)
    {
        string email = await DisplayPromptAsync("Recuperar Contraseña", "Ingresa tu correo electrónico registrado:", "Enviar", "Cancelar", keyboard: Keyboard.Email);

        if (!string.IsNullOrWhiteSpace(email))
        {
            var usuarioService = new UsuarioService();
            bool enviado = await usuarioService.RecuperarPasswordAsync(email.Trim());

            if (enviado)
            {
                await DisplayAlert("Éxito", "Se ha enviado un correo para restablecer tu contraseña.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo enviar el correo. Verifica que esté bien escrito o registrado.", "OK");
            }
        }
    }
}