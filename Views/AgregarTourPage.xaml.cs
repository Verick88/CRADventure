using CRadventure.Models;
using CRadventure.Services;

namespace CRadventure.Views;

public partial class AgregarTourPage : ContentPage
{
    private readonly TourService _tourService = new TourService();
    public AgregarTourPage()
	{
		InitializeComponent();
	}

    private async void GuardarTour_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Validaciones básicas
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
                // Conversión automática si se deja vacío (Tasa aproximada: 1 USD = 520 CRC)
                precioExt = Math.Round(precioNac / 520.0, 2);
            }

            int.TryParse(txtDuracion.Text, out int duracion);

            // Crear el modelo con los datos recolectados
            var nuevoTour = new TourModel
            {
                NombreLugar = txtNombre.Text.Trim(),
                ImagenUrl = string.IsNullOrWhiteSpace(txtImagenUrl.Text) ? "default_tour.png" : txtImagenUrl.Text.Trim(),
                Provincia = pickerProvincia.SelectedItem?.ToString() ?? "San José",
                Dificultad = pickerDificultad.SelectedItem?.ToString() ?? "Media",
                PrecioNacional = precioNac,
                PrecioExtranjero = precioExt,
                DuracionHoras = duracion > 0 ? duracion : 1,
                Idiomas = string.IsNullOrWhiteSpace(txtIdiomas.Text) ? "Español" : txtIdiomas.Text.Trim(),
                FechaHoraVisual = string.IsNullOrWhiteSpace(txtFecha.Text) ? "Fecha por definir" : txtFecha.Text.Trim(),
                DescripcionCorta = txtDescCorta.Text?.Trim() ?? string.Empty,
                DescripcionLarga = txtDescLarga.Text?.Trim() ?? string.Empty,
                PuntoEncuentro = txtPuntoEncuentro.Text?.Trim() ?? string.Empty
            };

            // Guardar en Firebase a través del servicio
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