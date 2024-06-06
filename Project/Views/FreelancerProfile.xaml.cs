using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Project.Tables;
using Project.Services;
using System.IO;
using System.Windows.Input;
using System.Linq;
using Xamarin.Forms.Internals;
using Xamarin.Essentials;
using System.ComponentModel;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreelancerProfile : ContentPage, INotifyPropertyChanged
    {
        private string _name;
        private string _fullName;
        private string _userName;
        private int _id;
        private string _freelancerType;
        public bool ___isFreelancerVisible;
        public bool _isFreelancerVisible
        {
            get { return ___isFreelancerVisible; }
            set
            {
                ___isFreelancerVisible = value;
                OnPropertyChanged("_isFreelancerVisible");
            }
        }
        public bool ___isnotFreelancerVisible;
        public bool _isnotFreelancerVisible
        {
            get { return ___isnotFreelancerVisible; }
            set
            {
                ___isnotFreelancerVisible = value;
                OnPropertyChanged("_isnotFreelancerVisible");
            }
        }
        private RegUserTable _userData;
        public ObservableCollection<ShowPosts> Items { get; set; }
        public ICommand DeleteCommand { get; }
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
        public FreelancerProfile(string userName, string fullName, string freelancerType, RegUserTable userData, bool isFreelancerVisible, int id, string name, bool IsAdmin)
        {
            InitializeComponent();

            _fullName = fullName;
            _userName = userName;
            _id = id;
            _name = name;
            _IsAdmin = IsAdmin;
            _freelancerType = freelancerType;
            _isFreelancerVisible = isFreelancerVisible;
            _isnotFreelancerVisible = !isFreelancerVisible;
            _userData = userData;

            DeleteCommand = new Command<ShowPosts>(DeletePost);
            OpenUrlCommand = new Command<string>(OpenUrl);

            if(_userData !=null && !string.IsNullOrWhiteSpace(_userData.ImagePic))
            {
                byte[] imageBytes = Convert.FromBase64String(_userData.ImagePic);
                ProfilePic.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }

            // Set labels
            FullNameLabel.Text = _fullName;
            UsernameLabel.Text = _userName;
            FreelancerTypeLabel.Text = _freelancerType;

            Items = new ObservableCollection<ShowPosts>();
            BindingContext = this;
        }

        public bool IsFreelancerVisible
        {
            get => _isFreelancerVisible;
            set
            {
                _isFreelancerVisible = value;
                OnPropertyChanged(nameof(IsFreelancerVisible));
                OnPropertyChanged(nameof(IsNotFreelancerVisible)); // Update dependent property
            }
        }

        public bool IsNotFreelancerVisible
        {
            get => _isnotFreelancerVisible;
            set
            {
                _isnotFreelancerVisible = value;
                OnPropertyChanged(nameof(IsNotFreelancerVisible));
            }
        }

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

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            UserRepository userRepository = new UserRepository(dbpath);
            var posts = await userRepository.GetClientPosts(_userData);
            Items.Clear();
            foreach (var post in posts)
            {
                var TempPost = new ShowPosts
                {
                    FreelancerId = post.FreelancerId,
                    Id = post.Id,
                    PostImage = post.PostImage,
                    IsClient = post.IsClient,
                    PostDetails = post.PostDetails,
                    PostLink = post.PostLink,
                    UserId = post.UserId,
                    IsLink = post.IsLink
                };
                if (!string.IsNullOrWhiteSpace(post.PostImage))
                {
                    byte[] imageBytes = Convert.FromBase64String(post.PostImage);
                    TempPost.ImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                Items.Add(TempPost);
            }
        }

        private async void ChatButton_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            await Navigation.PushAsync(new FreelancerChatList(_userData.Id, dbPath, _userData.Name_+" "+ _userData.LastName));
            IsBusy.IsVisible = false;
        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");

            if (answer)
            {
                await Navigation.PopToRootAsync();
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            try
            {
                await Navigation.PushAsync(new FreelancerCreatePosts(_userData));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            }
            IsBusy.IsVisible = false;
        }

        private async void DeletePost(ShowPosts showpost)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            if (showpost != null)
            {
                bool answer = await DisplayAlert("Delete Confirmation", "Are you sure you want to delete this job?", "Yes", "No");
                if (answer)
                {
                    var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                    UserRepository userRepository = new UserRepository(dbpath);
                    var NewTempPost = new Posts
                    {
                        FreelancerId = showpost.FreelancerId,
                        Id = showpost.Id,
                        PostImage = showpost.PostImage,
                        IsClient = showpost.IsClient,
                        PostDetails = showpost.PostDetails,
                        PostLink = showpost.PostLink,
                        UserId = showpost.UserId,
                        IsLink = showpost.IsLink
                    };
                    await userRepository.DeletePosts(NewTempPost);
                    var posts = await userRepository.GetClientPosts(_userData);
                    Items.Clear();
                    foreach (var post in posts)
                    {
                        var TempPost = new ShowPosts
                        {
                            FreelancerId = post.FreelancerId,
                            Id = post.Id,
                            PostImage = post.PostImage,
                            IsClient = post.IsClient,
                            PostDetails = post.PostDetails,
                            PostLink = post.PostLink,
                            UserId = post.UserId,
                            IsLink = post.IsLink
                        };
                        if (!string.IsNullOrWhiteSpace(post.PostImage))
                        {
                            byte[] imageBytes = Convert.FromBase64String(post.PostImage);
                            TempPost.ImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                        }
                        Items.Add(TempPost);
                    }
                }
            }
            IsBusy.IsVisible = false;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new JobHistory(_userData.Id));
            IsBusy.IsVisible = false;
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new CurrentJobs(_userData));
            IsBusy.IsVisible = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new RequestJob(_userData.Id, _id,_userData, _name));
            IsBusy.IsVisible = false;
        }
        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new ClientChat(_userData.Id, _id, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db"), _name,_userData.Name_+ " "+_userData.LastName, _userData.FreelancerType));
            IsBusy.IsVisible = false;
        }

        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new SettingsPage(_userData));
            IsBusy.IsVisible = false;
        }

        private async void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            await Navigation.PushAsync(new PaymentList(_userData));
            IsBusy.IsVisible = false;
        }
    }
}

