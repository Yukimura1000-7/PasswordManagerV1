using PasswordManagerV1.Services;
using PasswordManagerV1.Views;
using System.IO;

namespace PasswordManagerV1;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        SetAppTheme();

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var dbPath = Path.Combine(desktopPath, "passwords.db3");
        var databaseService = new Services.DatabaseService(dbPath);

        MainPage = new NavigationPage(new Views.LoginPage(databaseService));
    }

    private void SetAppTheme()
    {
        var currentTheme = Application.Current.RequestedTheme;
        Application.Current.UserAppTheme = currentTheme;
        UpdateTheme(currentTheme);
    }

    private void UpdateTheme(AppTheme theme)
    {
        // Здесь можно добавить логику темизации
    }
}