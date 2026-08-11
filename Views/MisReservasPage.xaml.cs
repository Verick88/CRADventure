using CRadventure.Models;
using CRadventure.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CRadventure.Views;

public partial class MisReservasPage : ContentPage
{
    private readonly ReservaService _reservaService = new();
    public ObservableCollection<ReservaVisual> MisReservas { get; set; } = new();
    public ICommand CancelarReservaCommand { get; }

    public MisReservasPage()
    {
        InitializeComponent();
        CancelarReservaCommand = new Command<ReservaVisual>(async (reserva) => await CancelarReserva(reserva));
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarMisReservas();
    }

    //Carga las reservas
    private async Task CargarMisReservas()
    {
        var usuarioAuth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current.CurrentUser;
        if (usuarioAuth == null) return;

        try
        {
            MisReservas.Clear(); //Limpia la lista actual
            var reservasObtenidas = await _reservaService.ObtenerReservasUsuarioAsync(usuarioAuth.Uid); //Llama al servicio de obtener reservas de un usuario

            foreach (var reserva in reservasObtenidas)
            {
                MisReservas.Add(reserva); //Recorre una a una cada reserva y las va agregando
            }

            if (MisReservas.Count == 0) //Si las reservas estan en 0, se muestra la lista vacia para que cargue la imagen
            {
                layoutVacio.IsVisible = true;
                listaReservas.IsVisible = false;
            }
            else  //Si hay reservas, muestra la lista de reservas
            {
                layoutVacio.IsVisible = false;
                listaReservas.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar reservas: {ex.Message}", "OK");
        }
    }

    //Metodo para cancelar una reserva
    private async Task CancelarReserva(ReservaVisual reserva)
    {
        bool confirmar = await DisplayAlert("Cancelar Reserva",
            $"¿Seguro que quieres cancelar {reserva.CantidadEntradas} tiquetes para {reserva.NombreLugar}?",
            "Sí", "No");

        if (!confirmar) return;

        try
        {
            await _reservaService.CancelarReservaAsync(reserva); //Llama al servicio de cancelar reserva

            await DisplayAlert("Cancelada", "Reserva cancelada y cupos devueltos.", "OK");
            await CargarMisReservas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cancelar: {ex.Message}", "OK");
        }
    }
}
