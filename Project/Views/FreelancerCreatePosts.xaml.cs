using Project.Tables;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreelancerCreatePosts : ContentPage
    {
        private RegUserTable _userData;
        public string ImageString { get; set; } = string.Empty;

        public FreelancerCreatePosts(RegUserTable userData)
        {
            InitializeComponent();
            BindingContext = this;
            FreelancerCheckBox.IsChecked = PostLink.IsVisible = false;
            ClientCheckBox.IsChecked = PostPhoto.IsVisible = true;
            _userData = userData;
        }

        private async void OnUploadPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var statusCamera = await CheckAndRequestPermissionAsync<Permissions.Camera>();
                var statusStorageRead = await CheckAndRequestPermissionAsync<Permissions.StorageRead>();
                var statusStorageWrite = await CheckAndRequestPermissionAsync<Permissions.StorageWrite>();

                if (statusCamera != PermissionStatus.Granted || statusStorageRead != PermissionStatus.Granted || statusStorageWrite != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to access camera or storage. Permissions are required to capture a photo.", "OK");
                    return;
                }

                var photo = await MediaPicker.PickPhotoAsync();

                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    stream.Close();

                    if (memoryStream.Length > 2 * 1024 * 1024)
                    {
                        await DisplayAlert("Error", "Image size must be less than 2MB.", "OK");
                        return;
                    }

                    byte[] imageBytes = memoryStream.ToArray();
                    ImageString = Convert.ToBase64String(imageBytes);

                    await DisplayAlert("Success", "Image captured successfully.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to capture image: " + ex.Message, "OK");
            }
        }

        private async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>() where T : Permissions.BasePermission, new()
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<T>();
            }
            return status;
        }

        private bool IsValidImage(string fileName)
        {
            string[] validExtensions = { ".jpg", ".jpeg", ".png" };
            return validExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        private async void CreateButton_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            if (ClientCheckBox.IsChecked && string.IsNullOrWhiteSpace(ImageString))
            {
                await DisplayAlert("Error", "Please fill in all required fields and provide either a photo or a link.", "OK");
            }
            else if (FreelancerCheckBox.IsChecked && string.IsNullOrWhiteSpace(EntryPostLink.Text))
            {
                await DisplayAlert("Error", "Please fill in all required fields and provide either a photo or a link.", "OK");
            }
            else if (string.IsNullOrWhiteSpace(EntryPostDescription.Text))
            {
                await DisplayAlert("Error", "Please fill in all required fields and provide either a photo or a link.", "OK");
            }
            else
            {
                var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                UserRepository userRepository = new UserRepository(dbpath);
                Posts TempPost = new Posts
                {
                    FreelancerId = _userData.Id,
                    IsClient = false,
                    PostDetails = EntryPostDescription.Text,
                    PostLink = FreelancerCheckBox.IsChecked ? EntryPostLink.Text : null,
                    PostImage = ClientCheckBox.IsChecked? ImageString : null,
                    IsLink = FreelancerCheckBox.IsChecked ? true : false,
                    UserId = 0
                };
                bool IsCreated = await userRepository.AddPost(TempPost);
                if (IsCreated)
                {
                    await DisplayAlert("Success", "Your post has been uploaded.", "OK");
                    await Navigation.PopAsync();

                }
                else
                {
                    await DisplayAlert("Error", "This post has been already exist", "OK");
                }
            }
            IsBusy.IsVisible = false;
        }

        private void ClientCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (ClientCheckBox.IsChecked)
            {
                FreelancerCheckBox.IsChecked = false;
                PostPhoto.IsVisible = true;
                PostLink.IsVisible = false;
            }
        }

        private void FreelancerCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (FreelancerCheckBox.IsChecked)
            {
                ClientCheckBox.IsChecked = false;
                PostPhoto.IsVisible = false;
                PostLink.IsVisible = true;
            }
        }
    }
}
