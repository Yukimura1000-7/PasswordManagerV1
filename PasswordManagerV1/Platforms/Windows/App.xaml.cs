using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace PasswordManagerV1.WinUI;

public partial class App : MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        // Получаем текущее окно
        var window = new Microsoft.UI.Xaml.Window();
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var id = Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = AppWindow.GetFromWindowId(id);

        if (appWindow != null)
        {
            // Устанавливаем размер окна (например, 800x600 пикселей)
            appWindow.Resize(new SizeInt32(400, 400));

            // Центрируем окно на экране
            var displayArea = DisplayArea.GetFromWindowId(id, DisplayAreaFallback.Nearest);
            var centerPoint = new PointInt32(
                (displayArea.WorkArea.Width - 400) / 2 + displayArea.WorkArea.X,
                (displayArea.WorkArea.Height - 400) / 2 + displayArea.WorkArea.Y
            );
            appWindow.Move(centerPoint);

            // Запрещаем изменение размера окна
            appWindow.SetPresenter(AppWindowPresenterKind.Default); // Фиксированный размер
        }
    }
}