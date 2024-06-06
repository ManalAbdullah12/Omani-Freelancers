using Xamarin.Forms;
using Project.Tables;
using System;
using System.IO;

namespace Project.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(RegUserTable userData)
        {
            InitializeComponent();

            // Create an instance of the view model and set it as the BindingContext
            BindingContext = new SettingsPageViewModel(userData);

        }
    }
}
