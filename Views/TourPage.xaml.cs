
using CRadventure.Models;
using CRadventure.Services;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using System.Collections.ObjectModel;

namespace CRadventure.Views;

public partial class TourPage : ContentPage
{
    private readonly TourService _tourService = new TourService();
    private readonly UsuarioModel _usuarioActual;

    // Lista original completa que traemos de Firebase
    private List<TourModel> _listaToursCompleta = new();

    // Colección observable enlazada a tu CollectionView en el XAML
    public ObservableCollection<TourModel> ToursFiltrados { get; set; } = new();

    // Propiedad para la barra de búsqueda enlazada al SearchBar
    private string _textoBusqueda;
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            _textoBusqueda = value;
            OnPropertyChanged();
            FiltrarTours(); 
        }
    }
    public TourPage()
    {
        InitializeComponent();
        _usuarioActual = SesionService.UsuarioActual;
        BindingContext = this;

        // Ocultar el botón si el usuario es cliente 
        if (_usuarioActual != null && (_usuarioActual.Rol == "guia" || _usuarioActual.Rol == "admin"))
        {
            btnAgregarTour.IsVisible = true;
        }
        else
        {
            btnAgregarTour.IsVisible = false;
        }

        CargarTours();
    }

    private string _filtroProvinciaSeleccionada;
    private string _filtroDificultadSeleccionada;

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Recarga los tours desde Firebase cada vez que regresas o entras a la página
        CargarTours();
    }

    //Metodo para cargar los tours desde firebase
    private async void CargarTours()
    {
        try
        {
            // Se obtiene la lista de tours desde TourService
            var listaTours = await _tourService.ObtenerTodosLosToursAsync();

            _listaToursCompleta.Clear();
            ToursFiltrados.Clear();

            // Validar si el usuario actual existe y verificar su valor de extranjero
            bool esExtranjero = _usuarioActual != null && _usuarioActual.EsExtranjero;

            foreach (var tour in listaTours)
            {
                // Lógica de los precios aplicando de forma segura si es extranjero
                tour.AplicarTarifa(esExtranjero);

                // Valida que idiomas no esté vacío
                if (!string.IsNullOrEmpty(tour.Idiomas))
                {
                    tour.IdiomasTexto = tour.Idiomas;
                }
                else
                {
                    tour.IdiomasTexto = "Sin idioma";
                }

                // Guardamos en ambas listas
                _listaToursCompleta.Add(tour);
                ToursFiltrados.Add(tour);
            }
        }
        catch (Exception ex)
        {
            // Error si no carga la información
            await DisplayAlert("Error", $"No se pudo cargar la información: {ex.Message}", "OK");
        }
    }

    private async void AgregarTour(object sender, EventArgs e)
    {
        if (_usuarioActual != null && (_usuarioActual.Rol == "guia" || _usuarioActual.Rol == "admin"))
        {
            await Shell.Current.GoToAsync(nameof(AgregarTourPage));
        }
        else
        {
            await DisplayAlert("Acceso", "Solo los guías pueden agregar tours", "OK");
        }
    }

    private async void ClickTour(object sender, SelectionChangedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ReservaPage));
    }

    // Método que se ejecuta al presionar el botón de la imagen de filtros
    private async void AbrirFiltros(object sender, EventArgs e)
    {
        string accion = await Shell.Current.DisplayActionSheet(
            "Filtrar tours por:",
            "Cancelar",
            null,
            " Provincia",
            " Dificultad",
            " Limpiar todos los filtros");

        if (accion == " Provincia")
        {
            string provincia = await Shell.Current.DisplayActionSheet(
                "Selecciona la provincia:", "Cancelar", null, "Puntarenas", "San José", "Alajuela", "Cartago", "Heredia", "Guanacaste", "Limón");

            if (provincia != "Cancelar" && provincia != null)
            {
                _filtroProvinciaSeleccionada = provincia;
                FiltrarTours();
            }
        }
        else if (accion == " Dificultad")
        {
            string dificultad = await Shell.Current.DisplayActionSheet(
                "Selecciona la dificultad:", "Cancelar", null, "Fácil", "Media", "Alta");

            if (dificultad != "Cancelar" && dificultad != null)
            {
                _filtroDificultadSeleccionada = dificultad;
                FiltrarTours();
            }
        }
        else if (accion == " Limpiar todos los filtros")
        {
            _textoBusqueda = string.Empty;
            OnPropertyChanged(nameof(TextoBusqueda));
            _filtroProvinciaSeleccionada = null;
            _filtroDificultadSeleccionada = null;

            FiltrarTours();
        }
    }

    // Lógica para filtrar
    private void FiltrarTours()
    {
        var resultados = _listaToursCompleta.AsEnumerable();

        //Filtro por barra de búsqueda 
        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            resultados = resultados.Where(t => t.NombreLugar != null && t.NombreLugar.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));
        }

        // Filtro por Provincia
        if (!string.IsNullOrWhiteSpace(_filtroProvinciaSeleccionada))
        {
            resultados = resultados.Where(t => t.Provincia != null && t.Provincia.Equals(_filtroProvinciaSeleccionada, StringComparison.OrdinalIgnoreCase));
        }

        // Filtro por Dificultad
        if (!string.IsNullOrWhiteSpace(_filtroDificultadSeleccionada))
        {
            resultados = resultados.Where(t => t.Dificultad != null && t.Dificultad.Equals(_filtroDificultadSeleccionada, StringComparison.OrdinalIgnoreCase));
        }


        // Actualizar coleccion tiempo real
        ToursFiltrados.Clear();
        foreach (var tour in resultados)
        {
            ToursFiltrados.Add(tour);
        }
    }
}