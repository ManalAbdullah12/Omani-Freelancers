using Project.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientCurrentJob : ContentPage
    {
        public RegUserTable userData;
        public RegUserTable _userData
        {
            get { return userData; }
            set
            {
                userData = value;
                OnPropertyChanged("_userData");
            }
        }
        public ObservableCollection<Jobs> Items { get; set; }
        public ClientCurrentJob(RegUserTable userData)
        {
            InitializeComponent();
            _userData = userData;
            Items = new ObservableCollection<Jobs>();
            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            UserRepository userRepository = new UserRepository(dbpath);
            var posts = await userRepository.GetAcceptedClientJobs(_userData.Id);
            Items.Clear();
            foreach (var post in posts)
            {
                Items.Add(post);
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            var frame = sender as Frame;
            if (frame != null)
            {
                var selectedItem = frame.BindingContext as Jobs;
                if (selectedItem != null)
                {
                    await Navigation.PushAsync(new ClientCurrentJobDetails(selectedItem));

                }
            }
            IsBusy.IsVisible = false;
        }
    }
}