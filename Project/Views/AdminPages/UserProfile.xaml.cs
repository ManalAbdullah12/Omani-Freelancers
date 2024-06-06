using Project.Tables;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfile : ContentPage
    {
        private bool _IsClient { get; set; } = false;
        public ObservableCollection<RegUserTable> UsersList { get; set; }
        public UserProfile(bool IsClient)
        {
            InitializeComponent();
            _IsClient = IsClient;
            UsersList = new ObservableCollection<RegUserTable>();
            BindingContext = this;
            if (IsClient)
                TopText.Text = "Client List";
            else
                TopText.Text = "Freelancer List";
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            UpdateRequest(false,null);
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            var frame = sender as Button;
            if (frame != null)
            {
                var selectedItem = frame.BindingContext as RegUserTable;
                if (selectedItem != null)
                {
                    bool answer = await DisplayAlert("Warning", "Are you sure you want to delete it?", "Yes", "No");

                    if (answer)
                    {
                        UpdateRequest(true, selectedItem);
                    }
                }
            }
            Isbusy.IsVisible = false;
        }

        private async void OnViewProfileClicked(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            var frame = sender as Button;
            if (frame != null)
            {
                var selectedItem = frame.BindingContext as RegUserTable;
                if (selectedItem != null)
                {
                    if(_IsClient)
                        await Navigation.PushAsync(new ClientProfile(selectedItem.UserName, selectedItem.Name_ + " " + selectedItem.LastName, selectedItem,false));
                    else
                        await Navigation.PushAsync(new FreelancerProfile(selectedItem.UserName, selectedItem.Name_ + " " + selectedItem.LastName, null, selectedItem, false, selectedItem.Id,selectedItem.UserName, false));

                }
            }
            Isbusy.IsVisible = false;
        }

        private async void UpdateRequest(bool IsDelete, RegUserTable regUserTable)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            var userRepository = new UserRepository(dbPath); // Provide the database path
            if (IsDelete)
                await userRepository.DeleteUser(regUserTable); // Call the method to update user data
            var db = new SQLiteConnection(dbPath);
            var MyList = db.Table<RegUserTable>().ToList();
            if (MyList != null || MyList.Count > 0)
            {
                UsersList.Clear();
                foreach (var post in MyList)
                {
                    if(_IsClient)
                    {
                        if (post.IsClient)
                            UsersList.Add(post);
                    }
                    else
                    {
                        if (!post.IsClient)
                            UsersList.Add(post);
                    }
                }
            }
        }
    }
}