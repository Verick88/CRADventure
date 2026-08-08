using CRadventure.Models;
using CRadventure.Services;

namespace CRadventure.Views;

[QueryProperty(nameof(TourId), "TourId")]
public partial class EditarGuiasDetallePage : ContentPage
{
    private readonly TourService _tourService = new TourService();

    private string _tourId;
    public string TourId
    {
        get
        {
            return _tourId;
        }
        set
        {
            _tourId = value;
            CargarTourPorId(_tourId);
        }
    }

    private TourModel _tourActual = new TourModel();
    public TourModel TourActual
    {
        get
        {
            return _tourActual;
        }
        set
        {
            _tourActual = value;
            OnPropertyChanged();
        }
    }

    public EditarGuiasDetallePage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void CargarTourPorId(string id)
    {
        try
        {
            var todos = await _tourService.ObtenerTodosLosToursAsync();
            var encontrado = todos.FirstOrDefault(t => t.Id == id);

            if (encontrado != null)
            {
                TourActual = encontrado;
            }
            else
            {
                await DisplayAlert("Error", "No se encontró el tour seleccionado.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirmar", "¿Deseas guardar los cambios de este tour?", "Sí", "No");
        if (!confirmar)
        {
            return;
        }

        try
        {
            await _tourService.ActualizarTourAsync(TourActual);

            await DisplayAlert("Éxito", "Tour actualizado correctamente.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Advertencia", "¿Estás seguro de eliminar este tour permanentemente?", "Sí, eliminar", "Cancelar");
        if (!confirmar)
        {
            return;
        }

        try
        {
            await _tourService.EliminarTourAsync(TourActual.Id);

            await DisplayAlert("Eliminado", "El tour ha sido eliminado.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
        }
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}