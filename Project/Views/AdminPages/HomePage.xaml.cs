using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            AdminImage.Source = ImageSource.FromResource("Project.Images.profile_picture.png");
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new AdminPages.RegistrationRequest());
            Isbusy.IsVisible = false;
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new AdminPages.UserProfile(true));
            Isbusy.IsVisible = false;
        }

        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new AdminPages.UserProfile(false));
            Isbusy.IsVisible = false;
        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");

            if (answer)
            {
                await Navigation.PopToRootAsync();
            }
        }
    }
}