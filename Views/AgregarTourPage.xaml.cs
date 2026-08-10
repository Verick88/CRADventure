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

    private async void Regresarbtn(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void GuardarTour(object sender, EventArgs e)
    {
        try
        {
            // Conversiones 
            int.TryParse(txtPlazas.Text, out int plazasIngresadas);
            double.TryParse(txtPrecioNacional.Text, out double precioNac);
            double.TryParse(txtPrecioExtranjero.Text, out double precioExt);
            int.TryParse(txtDuracion.Text, out int duracionHoras);

            // Se obtiene nombre y apellidos de la sesion actual
            string nombreGuia = $"{SesionService.UsuarioActual?.Nombre} {SesionService.UsuarioActual?.Apellidos}".Trim();


            if (string.IsNullOrWhiteSpace(nombreGuia))
            {
                nombreGuia = "Guía General";
            }

            //Obtener el correo del guia
            string emailGuia = SesionService.UsuarioActual?.Email ?? string.Empty;

            // Crear el modelo con los x:Name exactos del XAML
            var nuevoTour = new TourModel
            {
                NombreLugar = txtNombre.Text ?? string.Empty,
                ImagenUrl = txtImagenUrl.Text ?? string.Empty,
                Provincia = pickerProvincia.SelectedItem?.ToString() ?? "San José",
                Dificultad = pickerDificultad.SelectedItem?.ToString() ?? "Fácil",
                PrecioNacional = precioNac,
                PrecioExtranjero = precioExt,
                DuracionHoras = duracionHoras,
                Idiomas = txtIdiomas.Text ?? string.Empty,
                PlazasDisponibles = plazasIngresadas,
                GuiasAdicionales = txtGuiasAdicionales.Text ?? string.Empty,
                FechaHoraVisual = txtFecha.Text ?? "Fecha por definir",
                DescripcionCorta = txtDescCorta.Text ?? string.Empty,
                DescripcionLarga = txtDescLarga.Text ?? string.Empty,
                PuntoEncuentro = txtPuntoEncuentro.Text ?? string.Empty,
                GuiaEmail = emailGuia,
                GuiaAsociado = nombreGuia
            };

            // Se guarda en firebase
            await _tourService.AgregarTourAsync(nuevoTour);

            await DisplayAlert("Éxito", "Tour guardado correctamente", "OK");
            await Shell.Current.GoToAsync(".."); // Regresar a la pantalla anterior
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
        }
    }
}