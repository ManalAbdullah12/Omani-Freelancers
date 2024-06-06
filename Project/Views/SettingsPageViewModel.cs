using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Project.Tables;
using Xamarin.Essentials;

namespace Project.Views
{
    public class SettingsPageViewModel : INotifyPropertyChanged
    {
        private string _userName;
        private string _password;
        private string _confirmPassword;
        private string _name;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _IdCardNumber;
        private bool _isClient; // Add this field to store user type
        private string _freelancerType; // Add this field to store freelancer type

        private readonly RegUserTable _userData; // Make userData a class-level variable

        private ImageSource _profilePic;
        private ImageSource _idCardPic;
        public string Imagestring { get; private set; }
        public string IdCardImageString { get; private set; }

        public ImageSource ProfilePic
        {
            get => _profilePic;
            set
            {
                if (_profilePic != value)
                {
                    _profilePic = value;
                    OnPropertyChanged(nameof(ProfilePic));
                }
            }
        }

        public ImageSource IdCardPic
        {
            get => _idCardPic;
            set
            {
                if (_idCardPic != value)
                {
                    _idCardPic = value;
                    OnPropertyChanged(nameof(IdCardPic));
                }
            }
        }

        public ICommand ChangeProfilePicCommand { get; set; }
        public ICommand ChangeIdCardPicCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPageViewModel(RegUserTable userData)
        {
            // Initialize the class-level userData variable
            _userData = userData;

            // Pre-populate fields with existing user data
            _userName = userData.UserName;
            _password = userData.Password;
            _confirmPassword = userData.Password; // Confirm password assumes to be the same as password initially
            _name = userData.Name_;
            _lastName = userData.LastName;
            _email = userData.Email;
            _phoneNumber = userData.PhoneNumber;
            _isClient = userData.IsClient; // Store user type
            _freelancerType = userData.FreelancerType; // Store freelancer type
            _IdCardNumber = userData.IdCardNumber.ToString(); // Store freelancer type

            if (userData != null && !string.IsNullOrWhiteSpace(userData.ImagePic))
            {
                Imagestring = userData.ImagePic;
                byte[] imageBytes = Convert.FromBase64String(userData.ImagePic);
                ProfilePic = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }

            if (userData != null && !string.IsNullOrWhiteSpace(userData.IdCardPic))
            {
                IdCardImageString = userData.IdCardPic;
                byte[] imageBytes = Convert.FromBase64String(userData.IdCardPic);
                IdCardPic = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }

            // Initialize commands
            SaveChangesCommand = new Command(async () => await SaveChangesButton_Clicked());
            ChangeProfilePicCommand = new Command(OnChangeProfilePic);
            ChangeIdCardPicCommand = new Command(OnChangeIdCardPic);
        }

        private void OnChangeProfilePic()
        {
            OnUploadPhotoClicked();
        }
        
        private async void OnChangeIdCardPic()
        {
            try
            {
                var statusCamera = await CheckAndRequestPermissionAsync<Permissions.Camera>();
                var statusStorageRead = await CheckAndRequestPermissionAsync<Permissions.StorageRead>();
                var statusStorageWrite = await CheckAndRequestPermissionAsync<Permissions.StorageWrite>();

                if (statusCamera != PermissionStatus.Granted || statusStorageRead != PermissionStatus.Granted || statusStorageWrite != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert("Permission Denied", "Unable to access camera or storage. Permissions are required to capture a photo.", "OK");
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
                        await Application.Current.MainPage.DisplayAlert("Error", "Image size must be less than 2MB.", "OK");
                        return;
                    }

                    byte[] imageBytes = memoryStream.ToArray();
                    IdCardImageString = Convert.ToBase64String(imageBytes);
                    IdCardPic = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void OnUploadPhotoClicked()
        {
            try
            {
                var statusCamera = await CheckAndRequestPermissionAsync<Permissions.Camera>();
                var statusStorageRead = await CheckAndRequestPermissionAsync<Permissions.StorageRead>();
                var statusStorageWrite = await CheckAndRequestPermissionAsync<Permissions.StorageWrite>();

                if (statusCamera != PermissionStatus.Granted || statusStorageRead != PermissionStatus.Granted || statusStorageWrite != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert("Permission Denied", "Unable to access camera or storage. Permissions are required to capture a photo.", "OK");
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
                        await Application.Current.MainPage.DisplayAlert("Error", "Image size must be less than 2MB.", "OK");
                        return;
                    }

                    byte[] imageBytes = memoryStream.ToArray();
                    Imagestring = Convert.ToBase64String(imageBytes);
                    ProfilePic = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                }
            }
            catch (Exception ex)
            {
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


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand SaveChangesCommand { get; private set; }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }
        
        public string IdCardNumber
        {
            get { return _IdCardNumber; }
            set
            {
                if (_IdCardNumber != value)
                {
                    _IdCardNumber = value;
                    OnPropertyChanged(nameof(IdCardNumber));
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }

        public string Name_
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name_));
                }
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        public bool IsClient
        {
            get { return _isClient; }
            set
            {
                if (_isClient != value)
                {
                    _isClient = value;
                    OnPropertyChanged(nameof(IsClient));
                }
            }
        }

        public string FreelancerType
        {
            get { return _freelancerType; }
            set
            {
                if (_freelancerType != value)
                {
                    _freelancerType = value;
                    OnPropertyChanged(nameof(FreelancerType));
                }
            }
        }

        private async Task SaveChangesButton_Clicked()
        {
            // Perform validation...
            if (!IsPasswordValid())
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Password must contain letters and numbers.", "OK");
                return;
            }

            if (UserName == null || Email == null || Name_ == null || LastName == null || Password == null || ConfirmPassword == null ||
                UserName.Trim() == "" || Email.Trim() == "" || Name_.Trim() == "" || LastName.Trim() == "" || Password.Trim() == "" || ConfirmPassword.Trim() == "" || IdCardNumber.Trim() == "")
            {
                await Application.Current.MainPage.DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (!IsNameValid())
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Name and Last Name must contain letters only.", "OK");
                return;
            }

            if (!IsEmailValid())
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid email format.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if(Imagestring == null || string.IsNullOrWhiteSpace(Imagestring))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Image is required", "OK");
                return;
            }

            if (IdCardImageString == null || string.IsNullOrWhiteSpace(IdCardImageString))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Id Card Image is required", "OK");
                return;
            }

            if (UserName.Equals("Admin123"))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "This Account has already been Registered", "OK");
                return; // Remove this line to stay on the registration page
            }

            if (Password.Length < 8)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Password must be atleast 8 characters", "OK");
                return;
            }

            if (!PhoneNumber.All(char.IsDigit) || PhoneNumber.Length < 8)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Phone number must be in numbers and eight digits", "OK");
                return;
            }

            if (!(IdCardNumber.All(char.IsDigit)))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Card number must be in numbers", "OK");
                return;
            }

            // Update user data in the database...
            try
            {
                var updatedUserData = new RegUserTable
                {
                    ImagePic = Imagestring,
                    Id = _userData.Id, // Ensure the correct Id is used for updating
                    UserName = UserName,
                    Password = Password,
                    Name_ = Name_,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    IdCardPic = IdCardImageString,
                    IsClient = IsClient, // Preserve the user type
                    FreelancerType = FreelancerType,
                    IdCardNumber = Convert.ToInt32(IdCardNumber),
                    AccountStatus = _userData.AccountStatus,
                    UserID = _userData.UserID,
                    FollowersCount = _userData.FollowersCount,
                };

                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                var userRepository = new UserRepository(dbPath); // Provide the database path
                await userRepository.UpdateUserData(updatedUserData); // Call the method to update user data

                // Display success message
                await Application.Current.MainPage.DisplayAlert("Success", "Changes saved successfully. User Required to Login After Saving Changes", "OK");

                // Navigate back to the login page
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                // Display error message if saving changes fails
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save changes: {ex.Message}", "OK");
            }
        }

        private bool IsPasswordValid()
        {
            // Password must contain letters and numbers
            return !string.IsNullOrEmpty(Password) && Password.Any(char.IsLetter) && Password.Any(char.IsDigit);
        }

        private bool IsNameValid()
        {
            // Name and Last Name must contain letters only
            return !string.IsNullOrEmpty(Name_) && Name_.All(char.IsLetter) && LastName.All(char.IsLetter);
        }

        private bool IsEmailValid()
        {
            // Email format validation using regular expression
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return !string.IsNullOrEmpty(Email) && Regex.IsMatch(Email, emailPattern);
        }
    }
}
