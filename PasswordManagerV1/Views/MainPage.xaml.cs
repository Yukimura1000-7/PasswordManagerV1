using PasswordManagerV1.ViewModels;
using Microsoft.Maui;

namespace PasswordManagerV1.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Установка начальной темы
        UpdateTheme(Application.Current.RequestedTheme);
    }

    private void OnAccountSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Models.UserAccount account)
        {
            ((MainViewModel)BindingContext).CopyPasswordCommand.Execute(account.EncryptedPassword);
        }
    }

    private void OnThemeChanged(object sender, System.EventArgs e)
    {
        // Переключение темы
        var currentTheme = Application.Current.RequestedTheme;
        var newTheme = currentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
        Application.Current.UserAppTheme = newTheme;

        // Обновление интерфейса
        UpdateTheme(newTheme);
    }

    private void UpdateTheme(AppTheme theme)
    {
        if (theme == AppTheme.Light)
        {
            this.Style = (Style)Application.Current.Resources["LightTheme"];
            Resources["LabelStyle"] = Application.Current.Resources["LightLabel"];
            Resources["ButtonStyle"] = Application.Current.Resources["LightButton"];
        }
        else
        {
            this.Style = (Style)Application.Current.Resources["DarkTheme"];
            Resources["LabelStyle"] = Application.Current.Resources["DarkLabel"];
            Resources["ButtonStyle"] = Application.Current.Resources["DarkButton"];
        }
    }
}