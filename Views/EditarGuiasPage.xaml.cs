using System.Collections.ObjectModel;
using CRadventure.Models;
using CRadventure.Services;

namespace CRadventure.Views;

public partial class EditarGuiasPage : ContentPage
{
    private readonly TourService _tourService = new TourService();
    private List<TourModel> _todosLosTours = new();

    public ObservableCollection<TourModel> ToursList { get; set; } = new();

    private string _textoBusqueda = string.Empty;
    public string TextoBusqueda
    {
        get
        {
            return _textoBusqueda;
        }
        set
        {
            _textoBusqueda = value;
            OnPropertyChanged();
            Filtrar();
        }
    }

    public EditarGuiasPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarTours();
    }

    private async Task CargarTours()
    {
        try
        {
            var user = SesionService.UsuarioActual;
            if (user == null)
            {
                return;
            }

            var lista = await _tourService.ObtenerTodosLosToursAsync();
            bool esAdmin = user.Rol.Equals("admin", StringComparison.OrdinalIgnoreCase);

            if (esAdmin)
            {
                _todosLosTours = lista;
            }
            else
            {
                string nombreCompleto = $"{user.Nombre} {user.Apellidos}".Trim();
                _todosLosTours = lista.Where(t =>
                    (t.GuiaAsociado?.Trim().Equals(user.Uid, StringComparison.OrdinalIgnoreCase) == true) ||
                    (t.GuiaAsociado?.Trim().Equals(nombreCompleto, StringComparison.OrdinalIgnoreCase) == true)
                ).ToList();
            }

            foreach (var t in _todosLosTours)
            {
                t.AplicarTarifa(user.EsExtranjero);
            }

            Filtrar();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void Filtrar()
    {
        var resultados = _todosLosTours;

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            resultados = _todosLosTours.Where(t =>
                t.NombreLugar != null && t.NombreLugar.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        ToursList.Clear();
        foreach (var t in resultados)
        {
            ToursList.Add(t);
        }
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnTourSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TourModel tour)
        {
            return;
        }

        ((CollectionView)sender).SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(EditarGuiasDetallePage)}?TourId={tour.Id}");
    }
}