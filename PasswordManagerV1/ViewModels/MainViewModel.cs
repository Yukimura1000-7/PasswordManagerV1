using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using PasswordManagerV1.Models;
using PasswordManagerV1.Services;

namespace PasswordManagerV1.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        private readonly DatabaseService _databaseService;
        private readonly int _userId;

        public ObservableCollection<UserAccount> Accounts { get; set; } = new();

        public ICommand LoadAccountsCommand { get; }
        public ICommand AddAccountCommand { get; }
        public ICommand CopyPasswordCommand { get; }

        public MainViewModel(DatabaseService databaseService, int userId)
        {
            _databaseService = databaseService;
            _userId = userId;

            LoadAccountsCommand = new Command(async () => await LoadAccounts());
            AddAccountCommand = new Command(async () => await AddAccount());
            CopyPasswordCommand = new Command<string>(async (password) => await CopyToClipboard(password));

            LoadAccountsCommand.Execute(null);
        }
        private async Task LoadAccounts()
        {
            try
            {
                var accounts = await _databaseService.GetAccountsAsync(_userId);
                Console.WriteLine($"Loaded {accounts.Count} accounts for user {_userId}");

                Accounts.Clear();
                foreach (var account in accounts)
                {
                    Console.WriteLine($"Adding account: {account.ServiceName}, {account.Login}");
                    Accounts.Add(account);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading accounts: {ex.Message}");
            }
        }

        private async Task AddAccount()
        {
            // Запрашиваем у пользователя данные
            var serviceName = await Application.Current.MainPage.DisplayPromptAsync("Добавить аккаунт", "Название сервиса");
            var login = await Application.Current.MainPage.DisplayPromptAsync("Добавить аккаунт", "Логин");

            // Открываем диалог для ввода пароля
            var passwordDialog = new Views.PasswordDialog();
            await Application.Current.MainPage.Navigation.PushModalAsync(passwordDialog);

            while (!passwordDialog.IsConfirmed && !string.IsNullOrEmpty(passwordDialog.EnteredPassword))
            {
                await Task.Delay(100);
            }

            var password = passwordDialog.EnteredPassword;

            // Проверяем, что все поля заполнены
            if (!string.IsNullOrEmpty(serviceName) && !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                Console.WriteLine($"Adding account: ServiceName={serviceName}, Login={login}, Password={password}");

                // Шифруем пароль
                var encryptedPassword = EncryptionService.Encrypt(password);
                if (string.IsNullOrEmpty(encryptedPassword))
                {
                    Console.WriteLine("Encryption failed.");
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось зашифровать пароль.", "OK");
                    return;
                }

                // Создаем новую запись аккаунта
                var account = new Models.UserAccount
                {
                    UserId = _userId,
                    ServiceName = serviceName,
                    Login = login,
                    EncryptedPassword = encryptedPassword
                };

                // Сохраняем аккаунт в базу данных
                await _databaseService.SaveAccountAsync(account);
                Accounts.Add(account); // Добавляем запись в ObservableCollection
            }
            else
            {
                Console.WriteLine("Failed to add account: Some fields are empty.");
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Заполните все поля.", "OK");
            }
        }
        private async Task CopyToClipboard(string encryptedPassword)
        {
            var decryptedPassword = EncryptionService.Decrypt(encryptedPassword);
            await Clipboard.SetTextAsync(decryptedPassword);
            await Application.Current.MainPage.DisplayAlert("Успех", "Пароль скопирован в буфер обмена", "OK");
        }

    }


}