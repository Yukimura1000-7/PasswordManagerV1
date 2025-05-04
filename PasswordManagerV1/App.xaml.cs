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

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "passwords.db3");
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
        if (theme == AppTheme.Light)
        {
            Application.Current.Resources["LabelStyle"] = Application.Current.Resources["LightText"];
            Application.Current.Resources["EntryStyle"] = Application.Current.Resources["LightInput"];
            Application.Current.Resources["ButtonStyle"] = Application.Current.Resources["LightButton"];
        }
        else
        {
            Application.Current.Resources["LabelStyle"] = Application.Current.Resources["DarkText"];
            Application.Current.Resources["EntryStyle"] = Application.Current.Resources["DarkInput"];
            Application.Current.Resources["ButtonStyle"] = Application.Current.Resources["DarkButton"];
        }
    }
}