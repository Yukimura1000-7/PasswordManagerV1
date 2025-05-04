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
        private int _userId; // Добавляем поле для UserId

        public ObservableCollection<UserAccount> Accounts { get; set; } = new();

        public ICommand LoadAccountsCommand { get; }
        public ICommand AddAccountCommand { get; }
        public ICommand CopyPasswordCommand { get; }

        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public MainViewModel(DatabaseService databaseService, int userId)
        {
            _databaseService = databaseService;
            UserId = userId;

            LoadAccountsCommand = new Command(async () => await LoadAccounts());
            AddAccountCommand = new Command<string[]>(async (parameters) =>
            {
                if (parameters.Length == 3)
                {
                    await AddAccount(parameters[0], parameters[1], parameters[2]);
                }
            });
            CopyPasswordCommand = new Command<string>(async (password) => await CopyToClipboard(password));

            LoadAccountsCommand.Execute(null);
        }

        private async Task LoadAccounts()
        {
            var accounts = await _databaseService.GetAccountsAsync(UserId);
            Accounts.Clear(); // Очищаем коллекцию перед добавлением новых данных
            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }
        }
        public async Task SaveAccountAsync(UserAccount account)
        {
            await _databaseService.SaveAccountAsync(account);
            Accounts.Add(account);
        }

        private async Task AddAccount(string serviceName, string login, string password)
        {
            if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Заполните все поля.", "OK");
                return;
            }

            var encryptedPassword = Services.EncryptionService.Encrypt(password);
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось зашифровать пароль.", "OK");
                return;
            }

            var account = new Models.UserAccount
            {
                UserId = UserId,
                ServiceName = serviceName,
                Login = login,
                EncryptedPassword = encryptedPassword
            };

            await SaveAccountAsync(account);
            Accounts.Add(account);
        }

        // Добавьте свойства для хранения данных
        private string _serviceName;
        private string _login;
        private string _password;

        public string ServiceName
        {
            get => _serviceName;
            set => SetProperty(ref _serviceName, value);
        }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        private async Task CopyToClipboard(string encryptedPassword)
        {
            var decryptedPassword = Services.EncryptionService.Decrypt(encryptedPassword);
            await Clipboard.SetTextAsync(decryptedPassword);
            await Application.Current.MainPage.DisplayAlert("Успех", "Пароль скопирован в буфер обмена", "OK");
        }
    }
}