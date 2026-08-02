using CRadventure.Models;
using CRadventure.Services;
using Plugin.Firebase.Auth;

namespace CRadventure.Views;

public partial class PerfilPage : ContentPage
{
    // instanciamos nuestro servicio de usuarios y el servicio de autenticacion de Firebase
    private readonly UsuarioService _usuarioService;

    public PerfilPage()
    {
        InitializeComponent();
        _usuarioService = new UsuarioService();
    }

    // este metodo se ejecuta automaticamente cada vez que la pagina aparece en pantalla
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarDatosPerfilAsync();
    }

    // metodo para obtener el UID del usuario actual y buscar sus datos en firestore
    private async Task CargarDatosPerfilAsync()
    {
        try
        {
            // obtenemos el usuario autenticado actualmente en la app
            var usuarioAuth = CrossFirebaseAuth.Current.CurrentUser;
            if (usuarioAuth != null)
            {
                string uid = usuarioAuth.Uid;

                // llamamos a nuestro servicio usando el método seguro por UID
                var usuarioModel = await _usuarioService.ObtenerUsuarioPorUidAsync(uid);

                if (usuarioModel != null)
                {
                    // llenamos los campos visuales de la pantalla con la información de la base de datos
                    txtNombre.Text = usuarioModel.Nombre;
                    txtApellidos.Text = usuarioModel.Apellidos;
                    txtTelefono.Text = usuarioModel.Telefono;
                    lblEmail.Text = usuarioModel.Email; // por si tienes una etiqueta para mostrar el correo
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar el perfil: {ex.Message}", "OK");
        }
    }

    // evento que se ejecuta al presionar el botón de "Guardar Cambios" en el perfil
    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            var usuarioAuth = CrossFirebaseAuth.Current.CurrentUser;
            if (usuarioAuth == null) return;

            string uid = usuarioAuth.Uid;
            string nombre = txtNombre.Text;
            string apellidos = txtApellidos.Text;
            string telefono = txtTelefono.Text;

            // llamamos al servicio para actualizar los datos en Firestore
            await _usuarioService.ActualizarPerfilAsync(uid, nombre, apellidos, telefono);

            await DisplayAlert("Éxito", "Perfil actualizado correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
        }
    }

    // cerrar sesión desde el perfil
private async void OnCerrarSesion_Clicked(object sender, EventArgs e)
{
    bool confirmar = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de que deseas salir?", "Sí", "No");
    if (confirmar)
    {
        try
        {
            await CrossFirebaseAuth.Current.SignOutAsync();
            
            // destruye el Shel
            // Application.Current.MainPage = new NavigationPage(new LoginPage());

            // mantener el Shell vivo
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cerrar sesión: " + ex.Message, "OK");
        }
    }
}
}