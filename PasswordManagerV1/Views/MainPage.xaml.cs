using PasswordManagerV1.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Threading.Tasks;
using PasswordManagerV1.Models;
using PasswordManagerV1.ViewModels;

namespace PasswordManagerV1.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ViewModels.MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnAddAccountClicked(object sender, EventArgs e)
        {
            var serviceName = await Application.Current.MainPage.DisplayPromptAsync("Добавить аккаунт", "Название сервиса:");
            if (string.IsNullOrEmpty(serviceName))
            {
                await DisplayAlert("Ошибка", "Название сервиса не может быть пустым.", "ОК");
                return;
            }

            var login = await Application.Current.MainPage.DisplayPromptAsync("Добавить аккаунт", "Логин:");
            if (string.IsNullOrEmpty(login))
            {
                await DisplayAlert("Ошибка", "Логин не может быть пустым.", "ОК");
                return;
            }

            var password = await Application.Current.MainPage.DisplayPromptAsync("Добавить аккаунт", "Пароль:");
            if (string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Пароль не может быть пустым.", "ОК");
                return;
            }

            ((ViewModels.MainViewModel)BindingContext).AddAccountCommand.Execute(
                new string[] { serviceName, login, password }
            );

            await DisplayAlert("Успех", "Аккаунт успешно добавлен.", "ОК");
        }

        private async void OnAccountSelected(object sender, ItemTappedEventArgs e)
        {
            var account = e.Item as UserAccount;
            if (account == null) return;

            var result = await Application.Current.MainPage.DisplayActionSheet(
                "Действия",
                "Отмена",
                null,
                "Редактировать",
                "Копировать логин",
                "Копировать пароль"
            );

            switch (result)
            {
                case "Редактировать":
                    await EditAccount(account);
                    break;
                case "Копировать пароль":
                    try
                    {
                        var decryptedPassword = Services.EncryptionService.Decrypt(account.EncryptedPassword);
                        Clipboard.SetTextAsync(decryptedPassword);
                        await DisplayAlert("Успех", "Пароль скопирован в буфер обмена", "ОК");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Ошибка", $"Не удалось расшифровать пароль: {ex.Message}", "ОК");
                    }
                    break;
            }
        }



        private async Task EditAccount(UserAccount account)
        {
            var decryptedPassword = Services.EncryptionService.Decrypt(account.EncryptedPassword);

            var serviceName = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Название сервиса:", placeholder: account.ServiceName);
            var login = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Логин:", placeholder: account.Login);
            var password = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Пароль:", placeholder: decryptedPassword);

            if (!string.IsNullOrEmpty(serviceName) && !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                account.ServiceName = serviceName;
                account.Login = login;
                account.EncryptedPassword = Services.EncryptionService.Encrypt(password);

                var viewModel = (ViewModels.MainViewModel)BindingContext;
                await viewModel.SaveAccountAsync(account);
                await viewModel.LoadAccounts(); // Перезагрузить список
            }
        }
    }
}