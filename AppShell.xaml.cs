using CRadventure.Views;

namespace CRadventure
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ReservaPage), typeof(CRadventure.Views.ReservaPage));
        }
    }
}
