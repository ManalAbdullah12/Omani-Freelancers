using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Project.Tables;
using SQLite;
using Xamarin.Forms;

namespace Project.Views
{
    public partial class FreelancerSearch : ContentPage
    {
        private string _selectedFreelancerType;
        private string _name;
        private int id;

        public FreelancerSearch(string selectedFreelancerType, int ID,string name)
        {
            InitializeComponent();

            _selectedFreelancerType = selectedFreelancerType;
            _name = name;
            id = ID;

            // Query the database and populate the UI with freelancers of the selected type
            PopulateFreelancers();
        }

        private async void PopulateFreelancers()
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            // Get the database path
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");

            // Open database connection
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                // Query the database for freelancers of the selected type
                List<RegUserTable> freelancers = conn.Table<RegUserTable>().Where(u => u.FreelancerType == _selectedFreelancerType).ToList();

                if (freelancers.Count == 0)
                {
                    // Display "No Freelancers Found" message
                    var noFreelancersLabel = new Label
                    {
                        Text = "No Freelancers Found",
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    MainLayout.Children.Add(noFreelancersLabel);
                }
                else
                {
                    foreach (var freelancer in freelancers)
                    {
                        if (freelancer.AccountStatus != 1)
                            continue;
                        // Create UI elements for each freelancer
                        var image = new Image { WidthRequest = 60, HeightRequest = 60, Aspect = Aspect.AspectFill };
                        try
                        {
                            var imageBase64 = freelancer.ImagePic;
                            byte[] imageBytes = Convert.FromBase64String(imageBase64);
                            image.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                        }
                        catch (Exception ex)
                        {
                            // Log the exception
                            Console.WriteLine("Error loading image: " + ex.Message);
                        }
                        var nameLabel = new Label { Text = $"{freelancer.Name_} {freelancer.LastName}", FontSize = 20, FontAttributes = FontAttributes.Bold };
                        var usernameLabel = new Label { Text = $"{freelancer.UserName}", FontSize = 16 };
                        var viewButton = new Button { Text = "View", BackgroundColor = Color.FromHex("#8a3ab9"), TextColor = Color.White, FontSize = 9, FontAttributes = FontAttributes.Bold, WidthRequest = 60, HeightRequest = 30, CornerRadius = 15 };
                        viewButton.BindingContext = freelancer; // Set the freelancer as the BindingContext for the button
                        viewButton.Clicked += ViewProfileButton_Clicked;

                        var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(0, 20, 0, 0) };
                        stackLayout.Children.Add(image);
                        stackLayout.Children.Add(new StackLayout { Children = { nameLabel, usernameLabel } });
                        stackLayout.Children.Add(viewButton);

                        // Add the stack layout to the MainLayout
                        MainLayout.Children.Add(stackLayout);
                    }
                }
            }
            IsBusy.IsVisible = false;
        }

        private async void ViewProfileButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var freelancer = (RegUserTable)button.BindingContext; // Retrieve the freelancer object from the BindingContext
            await Navigation.PushAsync(new FreelancerProfile(freelancer.UserName, freelancer.Name_, freelancer.FreelancerType, freelancer, false, id, _name, true));
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back
        }
    }
}
