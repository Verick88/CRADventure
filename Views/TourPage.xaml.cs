
using CRadventure.Models;
using CRadventure.Services;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;

namespace CRadventure.Views;

public partial class TourPage : ContentPage
{
    private readonly TourService _tourService = new TourService();
    private readonly UsuarioModel _usuarioActual;

    public TourPage()
    {
        InitializeComponent();
        _usuarioActual = SesionService.UsuarioActual;
        CargarTours();
    }

    private async void CargarTours()
    {
        try
        {
            // Se obtiene la lista de tours desde TourService
            var listaTours = await _tourService.ObtenerTodosLosToursAsync();

            foreach (var tour in listaTours)
            {
                // Logica de los precios
                tour.AplicarTarifa(_usuarioActual.EsExtranjero);

                // Valida que idiomas no este vacio
                if (!string.IsNullOrEmpty(tour.Idiomas))
                {
                    tour.IdiomasTexto = tour.Idiomas;
                }
                else
                {
                    tour.IdiomasTexto = "Sin idioma";
                }
            }

            // Se asigna la lista a la vista despues de haber procesado los idiomas
            this.cvTours.ItemsSource = listaTours;
        }
        catch (Exception ex)
        {
            // Error si no carga la informacion
            await DisplayAlert("Error", $"No se pudo cargar la información: {ex.Message}", "OK");
        }
    }

    private async void OnAgregarTourClicked(object sender, EventArgs e)
    {
        if (_usuarioActual.Rol == "guia" || _usuarioActual.Rol == "admin")
            await DisplayAlert("Próximamente", "Ir a Agregar Tour Page", "OK");
        else
            await DisplayAlert("Acceso", "Solo los guías pueden agregar tours", "OK");
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is TourModel tourSeleccionado)
        {
            await DisplayAlert("Próximamente", "Ir a Tour Detalle Page", "OK");
            cvTours.SelectedItem = null;
        }
    }
}