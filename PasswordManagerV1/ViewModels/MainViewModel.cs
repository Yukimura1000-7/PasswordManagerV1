using System;
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
        private int _userId;

        public ObservableCollection<UserAccount> Accounts { get; set; } = new();

        public ICommand LoadAccountsCommand { get; }
        public ICommand AddAccountCommand { get; }

        public MainViewModel(DatabaseService databaseService, int userId)
        {
            _databaseService = databaseService;
            _userId = userId;

            LoadAccountsCommand = new Command(async () => await LoadAccounts());
            LoadAccountsCommand.Execute(null);

         

            AddAccountCommand = new Command<string[]>(async (parameters) =>
            {
                if (parameters.Length == 3)
                {
                    await AddAccount(parameters[0], parameters[1], parameters[2]);
                }
            });
        }

        public async Task LoadAccounts()
        {
            var accounts = await _databaseService.GetAccountsAsync(_userId);
            Accounts.Clear();
            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }
        }

        private async Task AddAccount(string serviceName, string login, string password)
        {
            if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Заполните все поля.", "ОК");
                return;
            }

            var encryptedPassword = Services.EncryptionService.Encrypt(password);
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось зашифровать пароль.", "ОК");
                return;
            }

            var account = new Models.UserAccount
            {
                UserId = _userId,
                ServiceName = serviceName,
                Login = login,
                EncryptedPassword = encryptedPassword
            };

            await _databaseService.SaveAccountAsync(account);
            Accounts.Add(account);
        }

        public async Task SaveAccountAsync(UserAccount account)
        {
            await _databaseService.SaveAccountAsync(account);
        }

        public async Task UpdateAccount(UserAccount account)
        {
            var index = Accounts.IndexOf(Accounts.FirstOrDefault(a => a.Id == account.Id));
            if (index >= 0)
            {
                Accounts.RemoveAt(index);
                Accounts.Insert(index, account);
            }
        }
    }
}