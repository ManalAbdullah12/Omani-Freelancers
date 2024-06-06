using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Project.Tables; // Assuming DatabaseHelper is in this namespace

namespace Project.DatabaseHelper
{
    public partial class QueryUsers : ContentPage
    {
        Project.Tables.DatabaseHelper databaseHelper; // Specify the full namespace

        public QueryUsers()
        {
            InitializeComponent();
            databaseHelper = new Project.Tables.DatabaseHelper("your_database_path_here"); // Initialize your DatabaseHelper instance
        }

        async void GetAllUsersButton_Clicked(object sender, EventArgs e)
        {
            // Call the GetAllUsers method from your DatabaseHelper instance
            List<RegUserTable> users = databaseHelper.GetAllUsers();

            // Display the retrieved user data
            foreach (var user in users)
            {
                await DisplayAlert("User Details", $"Username: {user.UserName}, Email: {user.Email}, Phone Number: {user.PhoneNumber}", "OK");
            }
        }
    }
}
