using Project.Tables;
using SQLite;
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
    public partial class RegistrationPage : ContentPage
    {
        private string Imagestring = "";
        private string IdImagestring = "";
        public RegistrationPage()
        {
            InitializeComponent();
            LogoImage.Source = ImageSource.FromResource("Project.Images.newlogo.png");
        }

        async void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            // Check if passwords match
            if (EntryUserPassword.Text != EntryConfirmPassword.Text)
            {
                PasswordErrorLabel.Text = "Passwords don't match";
                PasswordErrorLabel.IsVisible = true;
                await DisplayAlert("Error", "Passwords don't match", "OK");
                PasswordErrorLabel.IsVisible = false; // Hide the error label
                IsBusy.IsVisible = false;
                return; // Remove this line to stay on the registration page
            }
            
            if (string.IsNullOrWhiteSpace(Imagestring))
            {
                await DisplayAlert("Error", "Profile Pic must be required", "OK");
                IsBusy.IsVisible = false;
                return; // Remove this line to stay on the registration page
            }
            
            if (string.IsNullOrWhiteSpace(IdImagestring))
            {
                await DisplayAlert("Error", "Id Card Pic must be required", "OK");
                IsBusy.IsVisible = false;
                return;// Remove this line to stay on the registration page
            }

            // Check if all required entries are filled
            if (string.IsNullOrWhiteSpace(EntryUserName.Text) || string.IsNullOrWhiteSpace(EntryUserPassword.Text) ||
                string.IsNullOrWhiteSpace(EntryName.Text) || string.IsNullOrWhiteSpace(EntryLastName.Text) ||
                string.IsNullOrWhiteSpace(EntryUserEmail.Text) || string.IsNullOrWhiteSpace(EntryUserPhoneNumber.Text) || string.IsNullOrWhiteSpace(EntryCardNumber.Text))
            {
                // Display pop-up message if any required entry is empty
                await DisplayAlert("Error", "Please make sure all required entries are filled", "OK");
                IsBusy.IsVisible = false;
                return;
            }

            if (EntryUserName.Text.Equals("Admin123"))
            {
                await DisplayAlert("Error", "This Account has already been Registered", "OK");
                IsBusy.IsVisible = false;
                return; // Remove this line to stay on the registration page
            }

            // Check if password contains both letters and digits
            if (!ContainsLettersAndDigits(EntryUserPassword.Text))
            {
                await DisplayAlert("Error", "Password must contain both letters and digits", "OK");
                IsBusy.IsVisible = false;
                return;
            }

            if (EntryUserPassword.Text.Length < 8)
            {
                await DisplayAlert("Error", "Password must be atleast 8 characters", "OK");
                IsBusy.IsVisible = false;
                return;
            }

            // Check if Name and Last Name contain letters only
            if (!ContainsLettersOnly(EntryName.Text) || !ContainsLettersOnly(EntryLastName.Text))
            {
                await DisplayAlert("Error", "Name and Last Name must contain letters only", "OK");
                IsBusy.IsVisible = false;
                return;
            }

            // Check if email format is valid
            if (!IsValidEmail(EntryUserEmail.Text))
            {
                await DisplayAlert("Error", "Please enter a valid email address", "OK");
                IsBusy.IsVisible = false;
                return;
            }
            
            if (!EntryUserPhoneNumber.Text.All(char.IsDigit) || EntryUserPhoneNumber.Text.Length < 8)
            {
                await DisplayAlert("Error", "Phone number must be in numbers and eight digits", "OK");
                IsBusy.IsVisible = false;
                return;
            }
            
            if (!(EntryCardNumber.Text.All(char.IsDigit)))
            {
                await DisplayAlert("Error", "Card number must be in numbers", "OK");
                IsBusy.IsVisible = false;
                return;
            }

            if (!ClientCheckBox.IsChecked && !FreelancerCheckBox.IsChecked)
            {
                await DisplayAlert("Error", "Please tick that either you are freelancer or client", "OK");
                IsBusy.IsVisible = false;
                return;
            }

            // Check if username or email already exists
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            var db = new SQLiteConnection(dbpath);
            db.CreateTable<RegUserTable>();

            var existingUser = db.Table<RegUserTable>().FirstOrDefault(u => u.UserName == EntryUserName.Text || u.Email == EntryUserEmail.Text);
            if (existingUser != null)
            {
                await DisplayAlert("Error", "Username or email already exists. Please choose another.", "OK");
                IsBusy.IsVisible = false;
                return;
            }

            // If all validation passes, proceed with registration
            var item = new RegUserTable()
            {
                UserName = EntryUserName.Text,
                Password = EntryUserPassword.Text,
                Name_ = EntryName.Text,
                LastName = EntryLastName.Text,
                Email = EntryUserEmail.Text,
                ImagePic = Imagestring,
                IdCardPic = IdImagestring,
                IdCardNumber = Convert.ToInt32(EntryCardNumber.Text),
                PhoneNumber = EntryUserPhoneNumber.Text,
                IsClient = ClientCheckBox.IsChecked, // Set IsClient based on checkbox
                FreelancerType = FreelancerCheckBox.IsChecked ?
                    (FreelancerTypePicker.SelectedIndex != -1 ?
                        (FreelancerTypePicker.SelectedItem.ToString() == "Other" ?
                            (string.IsNullOrWhiteSpace(OtherFreelancerTypeEntry.Text) ? null : OtherFreelancerTypeEntry.Text)
                            : FreelancerTypePicker.SelectedItem.ToString())
                        : null)
                    : null
            };

            // Check if Freelancer checkbox is checked and a freelancer type is selected
            if (FreelancerCheckBox.IsChecked)
            {
                if (FreelancerTypePicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "Please select a Freelancer Type", "OK");
                    IsBusy.IsVisible = false;
                    return;
                }

                if (FreelancerTypePicker.SelectedItem.ToString() == "Other" && string.IsNullOrWhiteSpace(OtherFreelancerTypeEntry.Text))
                {
                    await DisplayAlert("Error", "Please specify the Freelancer Type", "OK");
                    IsBusy.IsVisible = false;
                    return;
                }

                // If Other is selected, set FreelancerType to the specified value
                item.FreelancerType = FreelancerTypePicker.SelectedItem.ToString() == "Other" ? OtherFreelancerTypeEntry.Text : FreelancerTypePicker.SelectedItem.ToString();
            }

            db.Insert(item);

            // Display success message and navigate to appropriate profile page
            await DisplayAlert("Congratulations", "User Registration Successful. Please wait until your account is verified by Admin", "OK");

            // Navigate to appropriate profile page based on user type
            //if (item.IsClient)
            //{
            //    await Navigation.PushAsync(new ClientProfile(item.Name_ + " " + item.LastName, "@" + item.UserName, item));
            //}
            //else
            //{
            //    await Navigation.PushAsync(new FreelancerProfile(item.UserName, item.Name_ + " " + item.LastName, item.FreelancerType, item));
            //}
            await Navigation.PopAsync();
            IsBusy.IsVisible = false;
        }

        async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void ClientCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (ClientCheckBox.IsChecked)
            {
                FreelancerCheckBox.IsChecked = false;
                FreelancerTypeStack.IsVisible = false;
                OtherFreelancerTypeStack.IsVisible = false;
            }
        }

        private void FreelancerCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (FreelancerCheckBox.IsChecked)
            {
                ClientCheckBox.IsChecked = false;
                FreelancerTypeStack.IsVisible = true;
            }
            else
            {
                FreelancerTypeStack.IsVisible = false;
                OtherFreelancerTypeStack.IsVisible = false;
            }
        }

        private void FreelancerTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FreelancerTypePicker.SelectedItem.ToString() == "Other")
            {
                OtherFreelancerTypeStack.IsVisible = true;
            }
            else
            {
                OtherFreelancerTypeStack.IsVisible = false;
            }
        }

        bool ContainsLettersAndDigits(string input)
        {
            return input.Any(char.IsLetter) && input.Any(char.IsDigit);
        }

        bool ContainsLettersOnly(string input)
        {
            return input.All(char.IsLetter);
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async void OnUploadIDPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                IdImagestring = await UploadPic();
            }
            catch { }
        }

        private async void OnUploadPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                Imagestring = await UploadPic();
            }
            catch { }
        }

        private async Task<string> UploadPic()
        {
            try
            {
                var statusCamera = await CheckAndRequestPermissionAsync<Permissions.Camera>();
                var statusStorageRead = await CheckAndRequestPermissionAsync<Permissions.StorageRead>();
                var statusStorageWrite = await CheckAndRequestPermissionAsync<Permissions.StorageWrite>();

                if (statusCamera != PermissionStatus.Granted || statusStorageRead != PermissionStatus.Granted || statusStorageWrite != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to access camera or storage. Permissions are required to capture a photo.", "OK");
                    return null;
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
                        return null;
                    }

                    await DisplayAlert("Success", "Image captured successfully.", "OK");
                    byte[] imageBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to capture image: " + ex.Message, "OK");
            }
            return null;
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
    }
}
