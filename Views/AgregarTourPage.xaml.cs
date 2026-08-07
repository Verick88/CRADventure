using CRadventure.Models;
using CRadventure.Services;

namespace CRadventure.Views;

public partial class AgregarTourPage : ContentPage
{
    private readonly TourService _tourService = new TourService();
    private FileResult _imagenSeleccionada; // Variable para almacenar la foto elegida

    public AgregarTourPage()
    {
        InitializeComponent();
    }

    // Método para abrir la galería del dispositivo
    private async void SeleccionarImagen_Clicked(object sender, EventArgs e)
    {
        try
        {
            _imagenSeleccionada = await MediaPicker.Default.PickPhotoAsync();

            if (_imagenSeleccionada != null)
            {
                var stream = await _imagenSeleccionada.OpenReadAsync();
                imgPreview.Source = ImageSource.FromStream(() => stream);
                imgPreview.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo seleccionar la imagen de la galería.", "OK");
        }
    }

    private async void GuardarTour_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Validaciones básicas requeridas
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrecioNacional.Text) ||
                pickerProvincia.SelectedItem == null)
            {
                await DisplayAlert("Campos incompletos", "Por favor completa al menos el título, la provincia y el precio nacional.", "OK");
                return;
            }

            // Conversión de datos numéricos
            double.TryParse(txtPrecioNacional.Text, out double precioNac);

            double precioExt = 0;
            if (!string.IsNullOrWhiteSpace(txtPrecioExtranjero.Text))
            {
                double.TryParse(txtPrecioExtranjero.Text, out precioExt);
            }
            else
            {
                // Conversión automática por defecto (Ej. 1 USD = 520 CRC)
                precioExt = Math.Round(precioNac / 520.0, 2);
            }

            int.TryParse(txtDuracion.Text, out int duracion);
            int.TryParse(txtPlazas.Text, out int plazas);

            // Manejo de la imagen: Si seleccionó archivo, simulamos o subimos la ruta. 
            // Como Firestore guarda texto, aquí puedes almacenar la ruta local o la URL si configuras Storage.
            string rutaImagenFinal = _imagenSeleccionada?.FullPath ?? "default_tour.png";

            var nuevoTour = new TourModel
            {
                NombreLugar = txtNombre.Text.Trim(),
                ImagenUrl = rutaImagenFinal,
                Provincia = pickerProvincia.SelectedItem?.ToString() ?? "San José",
                Dificultad = pickerDificultad.SelectedItem?.ToString() ?? "Media",
                PrecioNacional = precioNac,
                PrecioExtranjero = precioExt,
                DuracionHoras = duracion > 0 ? duracion : 1,
                Idiomas = string.IsNullOrWhiteSpace(txtIdiomas.Text) ? "Español" : txtIdiomas.Text.Trim(),
                FechaHoraVisual = string.IsNullOrWhiteSpace(txtFecha.Text) ? "Fecha por definir" : txtFecha.Text.Trim(),

                // Nuevos campos solicitados
                PlazasDisponibles = plazas > 0 ? plazas : 10,
                GuiaAsociado = SesionService.UsuarioActual?.Nombre ?? "Guía Principal",
                GuiasAdicionales = string.IsNullOrWhiteSpace(txtGuiasAdicionales.Text) ? "Ninguno" : txtGuiasAdicionales.Text.Trim(),

                // Descripciones y puntos de encuentro
                DescripcionCorta = txtDescCorta.Text?.Trim() ?? string.Empty,
                DescripcionLarga = txtDescLarga.Text?.Trim() ?? string.Empty,
                PuntoEncuentro = txtPuntoEncuentro.Text?.Trim() ?? string.Empty
            };

            // Guardar en Firestore a través del servicio
            await _tourService.AgregarTourAsync(nuevoTour);

            await DisplayAlert("Éxito", "El tour ha sido agregado correctamente.", "OK");
            await Shell.Current.GoToAsync(".."); // Regresar a la pantalla anterior
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar el tour: {ex.Message}", "OK");
        }
    }
}