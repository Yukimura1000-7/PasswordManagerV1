using PasswordManagerV1.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Logging.Abstractions;

namespace PasswordManagerV1.Views;

public partial class MainPage : ContentPage
{
    public MainPage(ViewModels.MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

    }

    private async void OnAddAccountClicked(object sender, EventArgs e)
    {
        var serviceName = ServiceNameEntry.Text;
        var login = LoginEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Ошибка", "Заполните все поля.", "OK");
            return;
        }

        // Вызываем AddAccountCommand с параметрами
        ((ViewModels.MainViewModel)BindingContext).AddAccountCommand.Execute(
            new string[] { serviceName, login, password }
        );

        // Очищаем поля ввода
        ServiceNameEntry.Text = string.Empty;
        LoginEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;

        // Перезагружаем данные
        ((ViewModels.MainViewModel)BindingContext).LoadAccountsCommand.Execute(null);

        await DisplayAlert("Успех", "Аккаунт успешно добавлен.", "OK");
    }
    private void OnAccountSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Models.UserAccount account)
        {
            ((ViewModels.MainViewModel)BindingContext).CopyPasswordCommand.Execute(account.EncryptedPassword);
        }
    }
}