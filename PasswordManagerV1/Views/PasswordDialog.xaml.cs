using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace PasswordManagerV1.Views
{
    public partial class PasswordDialog : ContentPage
    {
        public string EnteredPassword { get; private set; }
        public bool IsConfirmed { get; private set; }

        public PasswordDialog()
        {
            InitializeComponent();
        }

        private void OnConfirmClicked(object sender, System.EventArgs e)
        {
            EnteredPassword = PasswordEntry.Text;
            IsConfirmed = true;
            Navigation.PopModalAsync();
        }

        private async void OnCancelClicked(object sender, System.EventArgs e)
        {
            IsConfirmed = false;
            await Navigation.PopModalAsync();
        }
    }
}