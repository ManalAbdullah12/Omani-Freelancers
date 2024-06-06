using Project.Tables;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Project.Views
{
    public partial class PaymentPage : ContentPage
    {
        private Jobs _showpost;
        public PaymentPage(Jobs showpost)
        {
            InitializeComponent();
            _showpost = showpost;
        }

        private async void PayNow_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            bool isValid = ValidateFields();

            if (isValid)
            {
                var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                UserRepository userRepository = new UserRepository(dbpath);
                _showpost.IsPayment = true;
                await userRepository.UpdateJob(_showpost);
                await DisplayAlert("Congratulations!", "Payment is successful", "OK");
                await Task.WhenAll(
    Navigation.PopAsync(),
    Navigation.PopAsync(),
    Navigation.PopAsync()
);

            }
            else
            {
                await DisplayAlert("Error", "Please make sure all required fields are filled correctly", "OK");
            }
            IsBusy.IsVisible = false;
        }

        private bool ValidateFields()
        {
            // Validate if all mandatory fields are filled
            if (string.IsNullOrWhiteSpace(EntryCardNumber.Text) ||
                string.IsNullOrWhiteSpace(EntryCardholderName.Text) ||
                string.IsNullOrWhiteSpace(EntryExpiryDate.Text) ||
                string.IsNullOrWhiteSpace(EntryCVV.Text) ||
                string.IsNullOrWhiteSpace(EntryBillingAddress.Text) ||
                string.IsNullOrWhiteSpace(EntryCity.Text) ||
                string.IsNullOrWhiteSpace(EntryZipCode.Text))
            {
                return false;
            }

            return true;
        }
    }
}
