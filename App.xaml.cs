using CRadventure.Services;
using CRadventure.Views;
using Plugin.Firebase.Auth;

namespace CRadventure;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

}