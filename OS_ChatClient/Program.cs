using Avalonia;
using Avalonia.ReactiveUI;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp
{

    //TODO Build in TERMINAL
    // dotnet publish OS_ChatClient.csproj /p:Configuration=Release /p:Platform=AnyCPU /p:TargetFramework=netcoreapp3.1 /p:PublishDir=bin\Publish\netcoreapp3.1\linux-x64\ /p:SelfContained=true /p:RuntimeIdentifier=linux-x64


    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}