
using CRadventure.Models;
using CRadventure.Services;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using System.Collections.ObjectModel;

namespace CRadventure.Views;

public partial class TourPage : ContentPage
{
    private readonly TourService _tourService = new TourService(); //Instancia privada para usar el TourService
    private readonly UsuarioModel _usuarioActual; //Variable para almacenar la informacion del usuario que inicio sesion

    // Se trae la lista completa de los tours desde firebase
    private List<TourModel> _listaToursCompleta = new();

    // Sublista dinamica que se muestra y actualiza en tiempo real para aplicar filtros
    public ObservableCollection<TourModel> ToursFiltrados { get; set; } = new();

    // Almacena el texto que el usuario va escribiendo en el buscador
    private string _textoBusqueda;
    public string TextoBusqueda
    {
        get => _textoBusqueda; //Devuelve el valor guardado en _textoBusqueda
        set //Se activa automaticamente cada vez que el usuario borra o escribe algo
        {
            _textoBusqueda = value; //actualiza el _textoBusqueda con lo que el usuario escribio
            OnPropertyChanged(); //Avisa a la interfaz grafica que el texto ha cambiado
            FiltrarTours(); //filtra los tours
        }
    }
    public TourPage()
    {
        InitializeComponent();
        _usuarioActual = SesionService.UsuarioActual;
        BindingContext = this; //Se declara el binding

        // Ocultar el botón agregar tour si el usuario es cliente 
        if (_usuarioActual != null && (_usuarioActual.Rol == "guia" || _usuarioActual.Rol == "admin"))
        {
            btnAgregarTour.IsVisible = true;
        }
        else
        {
            btnAgregarTour.IsVisible = false;
        }

        // Ocultar el botón si el usuario es cliente 
        if (_usuarioActual != null && (_usuarioActual.Rol == "guia" || _usuarioActual.Rol == "admin"))
        {
            btnEditarGuias.IsVisible = true;
        }
        else
        {
            btnEditarGuias.IsVisible = false;
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

            //Se limpian las listas para evitar duplicados
            _listaToursCompleta.Clear();
            ToursFiltrados.Clear();

            // Valida si el usuario es extranjero
            bool esExtranjero = _usuarioActual != null && _usuarioActual.EsExtranjero;

            foreach (var tour in listaTours)
            {
                // Ocultar el tour si las plazas disponibles son iguales o menores a 0
                if (tour.PlazasDisponibles <= 0)
                    continue;

                // Lógica de los precios 
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
        if (e.CurrentSelection.FirstOrDefault() is TourModel tourSeleccionado)
        {
            var parametros = new Dictionary<string, object>
            {
                { "TourAMostrar", tourSeleccionado }
            };

            await Shell.Current.GoToAsync(nameof(ReservaPage), parametros);
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    // Metodo cuando se abren los filtros
    private async void AbrirFiltros(object sender, EventArgs e)
    {
        string accion = await Shell.Current.DisplayActionSheet( //Espera a que el usuario seleccione una opcion y la almacena en la variable accion
            "Filtrar tours por:",
            "Cancelar",
            null,  //Boton de destruccion 
            " Provincia",
            " Dificultad",
            " Limpiar todos los filtros");

        if (accion == " Provincia")
        {
            string provincia = await Shell.Current.DisplayActionSheet( //Espera a que el usuario selecciona una opcion y la almacena en provincia
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
        // Se parte desde la lista completa omitiendo agotados
        var resultados = _listaToursCompleta.Where(t => t.PlazasDisponibles > 0);

        //Filtro por barra de búsqueda 
        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
                                    //Toma la coleccion de tours donde conserva los elementos donde t cumple con la condicion
                                                                                               //Revisa si el nombre del lugar coincide con lo que el user escribio, no distringue entre mayusc y minusc
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
        ToursFiltrados.Clear(); //Se limpia la coleccion
        foreach (var tour in resultados)
        {
            ToursFiltrados.Add(tour); //Se agregan los tours
        }
    }

    private async void EditarGuias(object sender, EventArgs e)
    {
        if (_usuarioActual != null && (_usuarioActual.Rol == "guia" || _usuarioActual.Rol == "admin"))
        {
            await Shell.Current.GoToAsync(nameof(EditarGuiasPage));
        }
        else
        {
            await DisplayAlert("Acceso", "Solo los guías pueden agregar tours", "OK");
        }
    }
}