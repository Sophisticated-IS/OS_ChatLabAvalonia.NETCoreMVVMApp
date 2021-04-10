using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Os_ChatServer.Views
{
    public class ServerMainWindow : Window
    {
        public ServerMainWindow()
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
    }
}