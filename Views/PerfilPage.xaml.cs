using CRadventure.Models;
using CRadventure.Services;
using Plugin.Firebase.Auth;

namespace CRadventure.Views;

public partial class PerfilPage : ContentPage
{
    private readonly UsuarioService _usuarioService;

    public PerfilPage()
    {
        InitializeComponent();
        _usuarioService = new UsuarioService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarDatosPerfilAsync();
    }

    private async Task CargarDatosPerfilAsync()
    {
        try
        {
            var usuarioAuth = CrossFirebaseAuth.Current.CurrentUser;
            if (usuarioAuth != null)
            {
                string uid = usuarioAuth.Uid;

                var usuarioModel = await _usuarioService.ObtenerUsuarioPorUidAsync(uid);

                if (usuarioModel != null)
                {
                    // =========================================================
                    // CORRECCIÓN CLAVE 1: Guardar el usuario actual en la sesión global
                    // =========================================================
                    SesionService.UsuarioActual = usuarioModel;

                    txtNombre.Text = usuarioModel.Nombre;
                    txtApellidos.Text = usuarioModel.Apellidos;
                    txtTelefono.Text = usuarioModel.Telefono;
                    lblEmail.Text = usuarioModel.Email;

                    if (!string.IsNullOrEmpty(usuarioModel.Rol))
                    {
                        lblRolUsuario.Text = $"Rol: {usuarioModel.Rol.ToUpper()}";

                        if (usuarioModel.Rol.ToLower() == "admin" || usuarioModel.Rol.ToLower() == "guia" || usuarioModel.Rol.ToLower() == "guía")
                        {
                            btnPanelAdmin.IsVisible = true;
                        }
                        else
                        {
                            btnPanelAdmin.IsVisible = false;
                        }
                    }
                    else
                    {
                        lblRolUsuario.Text = "Rol: CLIENTE";
                        btnPanelAdmin.IsVisible = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar el perfil: {ex.Message}", "OK");
        }
    }

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

            await _usuarioService.ActualizarPerfilAsync(uid, nombre, apellidos, telefono);

            // Actualizar también en memoria los datos locales de la sesión
            if (SesionService.UsuarioActual != null)
            {
                SesionService.UsuarioActual.Nombre = nombre;
                SesionService.UsuarioActual.Apellidos = apellidos;
                SesionService.UsuarioActual.Telefono = telefono;
            }

            await DisplayAlert("Éxito", "Perfil actualizado correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
        }
    }

    private async void OnCambiarFotoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una nueva foto de perfil",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                var filePath = result.FullPath;
                imgPerfil.Source = ImageSource.FromFile(filePath);

                byte[] imageBytes;
                using (var stream = await result.OpenReadAsync())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }
                string base64Image = Convert.ToBase64String(imageBytes);

                var usuarioAuth = CrossFirebaseAuth.Current.CurrentUser;
                if (usuarioAuth != null)
                {
                    await _usuarioService.ActualizarFotoPerfilAsync(usuarioAuth.Uid, base64Image);
                    await DisplayAlert("Éxito", "Foto de perfil actualizada y guardada correctamente.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar la foto: {ex.Message}", "OK");
        }
    }

    private async void OnCerrarSesion_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de que deseas salir?", "Sí", "No");
        if (confirmar)
        {
            try
            {
                // 1. Cerrar sesión en Firebase
                await CrossFirebaseAuth.Current.SignOutAsync();

                // 2. Limpiar la sesión global por completo
                SesionService.UsuarioActual = null;

                // 3. DESTRUIR EL SHELL: Reemplazar la página principal por una nueva instancia del Login
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo cerrar sesión: " + ex.Message, "OK");
            }
        }
    }

    private async void OnPanelAdminClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminPage());
    }
}