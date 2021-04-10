using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels;

namespace Os_ChatServer.Views
{
    public class RegisterUserWindow : Window
    {
        public RegisterUserWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Window_OnClosing(object? sender, CancelEventArgs e)
        {
            var dt = DataContext as RegisterUserWindowVM;
            dt?.On_WindowClosing();
        }
    }
}