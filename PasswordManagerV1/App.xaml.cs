using PasswordManagerV1.Services;
using PasswordManagerV1.Views;
using System.IO;

namespace PasswordManagerV1;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        string dbPath;

        dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "passwords.db3");
        var databaseService = new DatabaseService(dbPath);

        MainPage = new NavigationPage(new LoginPage(databaseService));

    }
}