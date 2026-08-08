using CRadventure.Views;

namespace CRadventure
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

           
            Routing.RegisterRoute(nameof(AgregarTourPage), typeof(AgregarTourPage));
            Routing.RegisterRoute(nameof(ReservaPage), typeof(ReservaPage));
            Routing.RegisterRoute(nameof(EditarGuiasPage), typeof(EditarGuiasPage));
            Routing.RegisterRoute(nameof(EditarGuiasDetallePage), typeof(EditarGuiasDetallePage));
        }
    }
}
