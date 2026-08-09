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
    //sender = El botón de "Mostrar/Ocultar contraseña" al que acaban de hacer clic. 
    //EventArgs e = Los datos técnicos del clic (Requisito obligatorio para el sistema).
    private void Password_Clicked(object sender, EventArgs e) 
    {
        txtPassword.IsPassword = !txtPassword.IsPassword; //Invierte el estado del password (para verla)

        if (txtPassword.IsPassword)
        {
            btnOjo.Source = "ver.png";//Si el password no esta visible, se muestra la imagen de ver
        }
        else
        {
            btnOjo.Source = "ocultar.png"; //Si el password esta visible, se muestra la imagen de ocultar
        }
    }

    // Método principal de inicio de sesión con carga
    private async void IniciarViaje(object sender, EventArgs e)
    {
        // Validar campos (que el correo y el password no sean nulos)
        if (string.IsNullOrWhiteSpace(txtCorreo.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
            return;
        }

        // Se activa el indicador de carga
        btnIniciar.Text = "";
        loadingIndicator.IsVisible = true;
        loadingIndicator.IsRunning = true;
        btnIniciar.IsEnabled = false;

        try
        {
            var auth = CrossFirebaseAuth.Current;

            // Forzar cierre de sesión previo para evitar que arrastre datos anteriores (Cuando se cierra sesion)
            try
            {
                await auth.SignOutAsync(); //Ordena a Firebase que cierre la sesión activa del usuario en la nube.
            }
            catch
            {
                //Si no habia una sesion activa, se ignora el error
            }

            //Limpia el servicio de sesion en memoria
            SesionService.UsuarioActual = null;

            // Autenticar con el nuevo usuario
            var user = await auth.SignInWithEmailAndPasswordAsync(txtCorreo.Text.Trim(), txtPassword.Text);

            if (user == null)
            {
                await DisplayAlert("Error", "No se pudo autenticar el usuario", "OK");
                return;
            }

            //Crea una instancia del servicio de usuarios y busca en firestore si ya existe un documento con el Uid del usuario que acaba de iniciar sesión.
            var service = new UsuarioService();
            var usuario = await service.ObtenerUsuarioPorUidAsync(user.Uid);

            //Por si la autenticacion de firebase dejo entrar a un usuario pero su documento de firestore no existia, lo crea automaticamente
            if (usuario == null)
            {
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
            {
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Error", "Rol no reconocido", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error Técnico de Firebase", ex.Message, "OK");
        }
        finally
        {
            //Restaurar botón al finalizar 
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
            btnIniciar.Text = "Iniciar viaje";
            btnIniciar.IsEnabled = true;
        }
    }

    // Método para recuperar contraseña
    private async void OlvidastePassword(object sender, EventArgs e)
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