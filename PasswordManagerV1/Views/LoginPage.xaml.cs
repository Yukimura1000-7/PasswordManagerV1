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

    private async void OnLoginClicked(object sender, System.EventArgs e)
    {
        try
        {
            var userName = UserNameEntry.Text;
            var password = PasswordEntry.Text;

            // Проверка входных данных
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Введите имя пользователя и пароль", "OK");
                return;
            }

            // Поиск пользователя в базе данных
            var userProfile = await _databaseService.GetUserProfileAsync(userName);
            if (userProfile == null || EncryptionService.Decrypt(userProfile.EncryptedPassword) != password)
            {
                await DisplayAlert("Ошибка", "Неверное имя пользователя или пароль", "OK");
                return;
            }

            // Переход на главную страницу с выбранным пользователем
            var mainViewModel = new ViewModels.MainViewModel(_databaseService, userProfile.Id);
            await Navigation.PushAsync(new MainPage(mainViewModel));
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            Console.WriteLine($"Error during login: {ex.Message}");
            await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
        }
    }
    private async void OnRegisterClicked(object sender, System.EventArgs e)
    {
        try
        {
            var userName = UserNameEntry.Text;
            var password = PasswordEntry.Text;

            // Проверка входных данных
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Введите имя пользователя и пароль", "OK");
                return;
            }

            // Проверка существования пользователя
            var existingProfile = await _databaseService.GetUserProfileAsync(userName);
            if (existingProfile != null)
            {
                await DisplayAlert("Ошибка", "Пользователь с таким именем уже существует", "OK");
                return;
            }

            // Шифрование пароля
            var encryptedPassword = EncryptionService.Encrypt(password);
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                await DisplayAlert("Ошибка", "Не удалось зашифровать пароль", "OK");
                return;
            }

            // Создание нового профиля
            var newProfile = new Models.UserProfile
            {
                UserName = userName,
                EncryptedPassword = encryptedPassword
            };

            // Сохранение профиля
            await _databaseService.SaveUserProfileAsync(newProfile);
            await DisplayAlert("Успех", "Регистрация завершена", "OK");
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            Console.WriteLine($"Error during registration: {ex.Message}");
            await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
        }
    }
}