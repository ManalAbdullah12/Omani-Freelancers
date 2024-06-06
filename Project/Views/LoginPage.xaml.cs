using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Project.Tables; // Ensure RegUserTable is defined in this namespace or imported from the correct one

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            ShowWelcomePageWithDelay(); // Call the method to show the WelcomePage with delay
            LogoImage.Source = ImageSource.FromResource("Project.Images.newlogo.png");
        }

        private async void ShowWelcomePageWithDelay()
        {
            var welcomePage = new WelcomePage(); // Create an instance of the WelcomePage
            await Navigation.PushModalAsync(welcomePage); // Display the WelcomePage as a modal

            // Simulate a time-consuming operation (e.g., loading data)
            await Task.Delay(6000); // Example: Simulate a 6-second delay

            await Navigation.PopModalAsync(); // Dismiss the WelcomePage when the operation is done
        }

        async void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new RegistrationPage());
            IsBusy.IsVisible = false;
        }

        async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                IsBusy.IsVisible = true;
                await Task.Delay(500);
                if (EntryUser.Text.Equals("Admin123") && EntryPassword.Text.Equals("Admin123*"))
                {
                    await Navigation.PushAsync(new AdminPages.HomePage());
                }
                else
                {
                    var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                    var db = new SQLiteConnection(dbpath);
                    string NewUser = EntryUser.Text;
                    var myquery = db.Table<RegUserTable>().FirstOrDefault(u => u.UserName.Equals(NewUser) && u.Password.Equals(EntryPassword.Text));

                    if (myquery != null)
                    {
                        if (myquery.AccountStatus == 0)
                        {
                            await DisplayAlert("Error", "Please wait until account is verified by Admin", "OK");
                        }
                        else if (myquery.AccountStatus == 1)
                        {
                            // Check if the user is a client or a freelancer and navigate accordingly
                            if (myquery.IsClient)
                            {
                                await Navigation.PushAsync(new ClientProfile(myquery.UserName, $"{myquery.Name_} {myquery.LastName}", myquery, true));
                            }
                            else
                            {
                                await Navigation.PushAsync(new FreelancerProfile(myquery.UserName, $"{myquery.Name_} {myquery.LastName}", myquery.FreelancerType, myquery, true, 0, null, true));
                            }
                        }
                        else if (myquery.AccountStatus == 2)
                        {
                            await DisplayAlert("Error", "Your account has been blocked by admin", "OK");
                        }
                    }
                    else
                    {
                        // Display an alert for wrong username or password
                        await DisplayAlert("Error", "Wrong Username or Password", "OK");
                    }
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", "Please register your account first", "OK");
            }
            IsBusy.IsVisible = false;

        }

        async void ForgotPassword_Tapped(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new ForgetPasswordPage());
            IsBusy.IsVisible = false;
        }
    }
}
