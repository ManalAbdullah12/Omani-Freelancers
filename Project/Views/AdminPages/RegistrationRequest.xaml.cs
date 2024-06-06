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
    public partial class RegistrationRequest : ContentPage
    {
        public ObservableCollection<RegUserTable> UsersList { get; set; }

        public RegistrationRequest()
        {
            InitializeComponent();
            UsersList = new ObservableCollection<RegUserTable>();
            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            UpdateRequest(false, true, null);
        }

        private async void OnAcceptClicked(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            var frame = sender as Button;
            if (frame != null)
            {
                var selectedItem = frame.BindingContext as RegUserTable;
                if (selectedItem != null)
                {
                    selectedItem.AccountStatus = 1;
                    if (Tab1.BackgroundColor == Color.Gray)
                        UpdateRequest(true, true, selectedItem);
                    else
                        UpdateRequest(true, false, selectedItem);
                }
            }
            Isbusy.IsVisible = false;
        }

        private async void OnRejectClicked(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true; 
            await Task.Delay(500);
            var frame = sender as Button;
            if (frame != null)
            {
                var selectedItem = frame.BindingContext as RegUserTable;
                if (selectedItem != null)
                {
                    selectedItem.AccountStatus = 2;
                    if(Tab1.BackgroundColor == Color.Gray)
                        UpdateRequest(true, true, selectedItem);
                    else
                        UpdateRequest(true, false, selectedItem);
                }
            }
            Isbusy.IsVisible = false;
        }

        private void Tab1_Clicked(object sender, EventArgs e)
        {
            Tab1.BackgroundColor = Color.Gray;
            Tab2.BackgroundColor = Color.LightGray;
            UpdateRequest(false, true, null);
        }

        private void Tab2_Clicked(object sender, EventArgs e)
        {
            Tab1.BackgroundColor = Color.LightGray;
            Tab2.BackgroundColor = Color.Gray;
            UpdateRequest(false, false, null);
        }

        private async void UpdateRequest(bool IsUpdate, bool IsClient, RegUserTable regUserTable)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            var userRepository = new UserRepository(dbPath); // Provide the database path
            if (IsUpdate) 
                await userRepository.UpdateUserData(regUserTable); // Call the method to update user data
            var db = new SQLiteConnection(dbPath);
            var MyList = db.Table<RegUserTable>().ToList();
            if (MyList != null || MyList.Count > 0)
            {
                UsersList.Clear();
                foreach (var post in MyList)
                {
                    if ((IsClient))
                    {
                        if(post.IsClient && post.AccountStatus == 0)
                            UsersList.Add(post);
                    }
                    else
                    {
                        if (!post.IsClient && post.AccountStatus == 0)
                            UsersList.Add(post);
                    }
                }
            }
        }
    }
}