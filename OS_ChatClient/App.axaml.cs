using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels;
using Os_ChatServer.Views;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var registerUserWindow = new RegisterUserWindow
            {
                DataContext = new RegisterUserWindowVM()
            };
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = registerUserWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}