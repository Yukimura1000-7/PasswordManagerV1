using Microsoft.Maui.Controls;

namespace PasswordManagerV1.Views;

public partial class AccountDialog : ContentPage
{
    public string ServiceName { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; }
    public bool IsConfirmed { get; private set; }

    public AccountDialog()
    {
        InitializeComponent();
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        ServiceName = ServiceNameEntry.Text;
        Login = LoginEntry.Text;
        Password = PasswordEntry.Text;
        IsConfirmed = true;
        Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        IsConfirmed = false;
        await Navigation.PopModalAsync();
    }
}