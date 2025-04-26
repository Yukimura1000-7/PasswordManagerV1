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

            // �������� ������� ������
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("������", "������� ��� ������������ � ������", "OK");
                return;
            }

            // ����� ������������ � ���� ������
            var userProfile = await _databaseService.GetUserProfileAsync(userName);
            if (userProfile == null || EncryptionService.Decrypt(userProfile.EncryptedPassword) != password)
            {
                await DisplayAlert("������", "�������� ��� ������������ ��� ������", "OK");
                return;
            }

            // ������� �� ������� �������� � ��������� �������������
            var mainViewModel = new ViewModels.MainViewModel(_databaseService, userProfile.Id);
            await Navigation.PushAsync(new MainPage(mainViewModel));
        }
        catch (Exception ex)
        {
            // ��������� ������
            Console.WriteLine($"Error during login: {ex.Message}");
            await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
        }
    }
    private async void OnRegisterClicked(object sender, System.EventArgs e)
    {
        try
        {
            var userName = UserNameEntry.Text;
            var password = PasswordEntry.Text;

            // �������� ������� ������
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("������", "������� ��� ������������ � ������", "OK");
                return;
            }

            // �������� ������������� ������������
            var existingProfile = await _databaseService.GetUserProfileAsync(userName);
            if (existingProfile != null)
            {
                await DisplayAlert("������", "������������ � ����� ������ ��� ����������", "OK");
                return;
            }

            // ���������� ������
            var encryptedPassword = EncryptionService.Encrypt(password);
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                await DisplayAlert("������", "�� ������� ����������� ������", "OK");
                return;
            }

            // �������� ������ �������
            var newProfile = new Models.UserProfile
            {
                UserName = userName,
                EncryptedPassword = encryptedPassword
            };

            // ���������� �������
            await _databaseService.SaveUserProfileAsync(newProfile);
            await DisplayAlert("�����", "����������� ���������", "OK");
        }
        catch (Exception ex)
        {
            // ��������� ������
            Console.WriteLine($"Error during registration: {ex.Message}");
            await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
        }
    }
}