using System;
using Xamarin.Forms;
using Project.Tables;
using Project.Services;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace Project.Views
{
    public partial class ClientProfile : ContentPage
    {
        private string _fullName;
        private string _userName;
        private RegUserTable _userData;
        public ObservableCollection<ShowPosts> Items { get; set; }
        public ICommand OpenUrlCommand { get; }

        private bool ___IsAdmin;
        public bool _IsAdmin
        {
            get { return ___IsAdmin; }
            set
            {
                ___IsAdmin = value;
                OnPropertyChanged("_IsAdmin");
            }
        }
        public ClientProfile(string userName, string fullName, RegUserTable userData, bool IsAdmin)
        {
            InitializeComponent();

            _fullName = fullName;
            _userName = userName;
            _userData = userData;
            _IsAdmin = IsAdmin;
            // Set labels
            FullNameLabel.Text = _fullName;
            UsernameLabel.Text = _userName;

            if (_userData != null && !string.IsNullOrWhiteSpace(_userData.ImagePic))
            {
                byte[] imageBytes = Convert.FromBase64String(_userData.ImagePic);
                ProfilePic.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }

            OpenUrlCommand = new Command<string>(OpenUrl);

            Items = new ObservableCollection<ShowPosts>();
            BindingContext = this;
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
        //    UserRepository userRepository = new UserRepository(dbpath);
        //    var posts = await userRepository.GetClientPosts();
        //    Items.Clear();
        //    foreach (var post in posts)
        //    {
        //        var TempPost = new ShowPosts
        //        {
        //            FreelancerId = post.FreelancerId,
        //            Id = post.Id,
        //            PostImage = post.PostImage,
        //            IsAccepted = post.IsAccepted,
        //            IsClient = post.IsClient,
        //            PostDetails = post.PostDetails,
        //            PostLink = post.PostLink,
        //            UserId = post.UserId,
        //            IsSubmitted = post.IsSubmitted,
        //            IsLink = post.IsLink
        //        };
        //        if (!string.IsNullOrWhiteSpace(post.PostImage))
        //        {
        //            byte[] imageBytes = Convert.FromBase64String(post.PostImage);
        //            TempPost.ImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        //        }
        //        Items.Add(TempPost);
        //    }
        //}

        private async void OpenUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Unable to open URL: " + ex.Message, "OK");
                }
            }
        }

        private async void ChatButton_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            // Open chat window or navigate to chat page
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
           await Navigation.PushAsync(new ClientChatList(_userData.Id, dbPath, _userData.Name_+" "+_userData.LastName)); // Convert userId to string and pass dbPath
            IsBusy.IsVisible = false;
        }

        private async void SearchButton_Clicked(object sender, EventArgs e)
        {
            // Check if a freelancer type is selected
            if (FreelancerTypePicker.SelectedItem == null || (FreelancerTypePicker.SelectedItem.ToString() == "Other" && string.IsNullOrWhiteSpace(OtherFreelancerTypeEntry.Text)))
            {
                // Display error message if no freelancer type is selected
                await DisplayAlert("Error", "Please select a freelancer type.", "OK");
                return;
            }

            // Get the selected freelancer type
            string selectedFreelancerType = FreelancerTypePicker.SelectedItem.ToString();
            if (selectedFreelancerType == "Other")
            {
                // Use the specified freelancer type if "Other" is selected
                selectedFreelancerType = OtherFreelancerTypeEntry.Text;
            }

            // Navigate to FreelancerSearchPage with the selected freelancer type
            await Navigation.PushAsync(new FreelancerSearch(selectedFreelancerType, _userData.Id, _userData.Name_ + " "+_userData.LastName));
        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            // Display confirmation dialog
            bool answer = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");

            // If user confirms logout, navigate back to login page
            if (answer)
            {
                // Navigate back to login page
                await Navigation.PopToRootAsync();
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new ClientJobHistory(_userData));
            IsBusy.IsVisible = false;
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new ClientCurrentJob(_userData));
            IsBusy.IsVisible = false;

        }

        private async void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new SettingsPage(_userData));
            IsBusy.IsVisible = false;

        }

        private async void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new PaymentList(_userData));
            IsBusy.IsVisible = false;
        }
    }
}
