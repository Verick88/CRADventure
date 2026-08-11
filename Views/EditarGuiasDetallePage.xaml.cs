using CRadventure.Models;
using CRadventure.Services;

namespace CRadventure.Views;

[QueryProperty(nameof(TourId), "TourId")]
public partial class EditarGuiasDetallePage : ContentPage
{
    private readonly TourService _tourService = new TourService(); //Inicializa la instancia del servicio

    private string _tourId;
    public string TourId
    {
        get
        {
            return _tourId; //Ver el tour actual
        }
        set //Cuando algo cambia
        {
            _tourId = value; //Guarda el ID que se le acaba de pasar
            CargarTourPorId(_tourId); //Va a Firebase por medio del metodo y busca los datos con ese ID
        }
    }

    private TourModel _tourActual = new TourModel();
    public TourModel TourActual
    {
        get
        {
            return _tourActual; //Ver el tour actual
        }
        set //Cuando algo cambia
        {
            _tourActual = value; //Guarda el nuevo dato en memoria
            OnPropertyChanged();//Cambia lo que se muestra en tiempo real
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
            var todos = await _tourService.ObtenerTodosLosToursAsync(); //Servicio para obtener los tours
            var encontrado = todos.FirstOrDefault(t => t.Id == id); //Busca en todos los tours hasta coincidir el id

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

    //Metodo para guardar el tour editado
    private async void GuardarClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirmar", "¿Deseas guardar los cambios de este tour?", "Sí", "No");
        if (!confirmar)
        {
            return;
        }

        try
        {
            await _tourService.ActualizarTourAsync(TourActual); //Llama al servicio para actualizar tours

            await DisplayAlert("Éxito", "Tour actualizado correctamente.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
        }
    }

    //Metodo para eliminar tour
    private async void EliminarClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Advertencia", "¿Estás seguro de eliminar este tour permanentemente?", "Sí, eliminar", "Cancelar");
        if (!confirmar)
        {
            return;
        }

        try
        {
            await _tourService.EliminarTourAsync(TourActual.Id); //Llama al servicio para eliminar por id

            await DisplayAlert("Eliminado", "El tour ha sido eliminado.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
        }
    }

    private async void VolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}