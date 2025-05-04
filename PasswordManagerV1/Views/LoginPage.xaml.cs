using PasswordManagerV1.Models;
using PasswordManagerV1.Services;
using Microsoft.Maui.Controls;

namespace PasswordManagerV1.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public LoginPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var userName = UserNameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Ошибка", "Введите имя пользователя и пароль", "OK");
            return;
        }

        var userProfile = await _databaseService.GetUserProfileAsync(userName);
        if (userProfile == null || Services.EncryptionService.Decrypt(userProfile.EncryptedPassword) != password)
        {
            await DisplayAlert("Ошибка", "Неверное имя пользователя или пароль", "OK");
            return;
        }

        var mainViewModel = new ViewModels.MainViewModel(_databaseService, userProfile.Id);
        await Navigation.PushAsync(new MainPage(mainViewModel));

        UserNameEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var userName = UserNameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Ошибка", "Введите имя пользователя и пароль", "OK");
            return;
        }

        var existingProfile = await _databaseService.GetUserProfileAsync(userName);
        if (existingProfile != null)
        {
            await DisplayAlert("Ошибка", "Пользователь с таким именем уже существует", "OK");
            return;
        }

        var encryptedPassword = Services.EncryptionService.Encrypt(password);
        var newProfile = new Models.UserProfile
        {
            UserName = userName,
            EncryptedPassword = encryptedPassword
        };

        await _databaseService.SaveUserProfileAsync(newProfile);
        await DisplayAlert("Успех", "Регистрация завершена", "OK");

        UserNameEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }

}