using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Services;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Views
{
    public class UserNameDialog : Window
    {
        public UserNameDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public void SetDataContextWithArgs(TcpClientSender clientSender)
        {
            DataContext = new UserNameDialogVM(this,clientSender);
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}