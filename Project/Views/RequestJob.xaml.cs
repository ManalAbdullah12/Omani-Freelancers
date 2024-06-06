using Project.Tables;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Project.Views
{
    public partial class RequestJob : ContentPage
    {
        private int _freelancerid;
        private RegUserTable userdata;
        private string FreelancerFullName;
        private int _id;

        public RequestJob(int freelancerid, int id,RegUserTable regUserTable,string freelancername)
        {
            InitializeComponent();
            BindingContext = this;
            _freelancerid = freelancerid;
            userdata = regUserTable;
            FreelancerFullName = freelancername;
            _id = id;
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTimeline = ((Picker)sender).SelectedItem as string;
            // Perform actions based on the selected timeline
            DisplayAlert("Selection Changed", $"Selected Timeline: {selectedTimeline}", "OK");
        }

        private async void SubmitJob_Clicked(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            if (string.IsNullOrWhiteSpace(EntryUserPhoneNumber.Text) || string.IsNullOrWhiteSpace(EntryCardNumber.Text) || string.IsNullOrWhiteSpace(EntryJobTitle.Text) || string.IsNullOrWhiteSpace(EntryJobDescription.Text) || PickerTimeline.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please make sure all required fields are filled", "OK");
            }
            else
            {
                if (!EntryUserPhoneNumber.Text.All(char.IsDigit) || EntryUserPhoneNumber.Text.Length < 8)
                {
                    await DisplayAlert("Error", "Phone number must be in numbers and eight digits", "OK");
                    Isbusy.IsVisible = false;
                    return;
                }

                if (!(EntryCardNumber.Text.All(char.IsDigit)))
                {
                    await DisplayAlert("Error", "Card number must be in numbers", "OK");
                    Isbusy.IsVisible = false;
                    return;
                }

                Jobs jobs = new Jobs{ FreelancerId = _freelancerid,UserId = _id, JobTitle = EntryJobTitle.Text, JobDescription = EntryJobDescription.Text, Price = Convert.ToDouble(Price.Text), StartingDate = CalendarPicker.Date.ToString(), TimeLine = PickerTimeline.SelectedItem.ToString(), UserFullName = FreelancerFullName, FreelancerFullName = userdata.Name_ + " " + userdata.LastName, IDcardNumber = EntryCardNumber.Text, PhoneNumber = EntryUserPhoneNumber.Text };
                var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                UserRepository userRepository = new UserRepository(dbpath);
                var result = await userRepository.AddJobs(jobs);
                Isbusy.IsVisible = false;
                if (result)
                {
                    await DisplayAlert("Success", "Job Request Submitted Successfully", "OK");
                    await Navigation.PopAsync();
                }
                else
                    await DisplayAlert("Error", "Job Request Submitted Unsuucessfully", "OK");
            }
            Isbusy.IsVisible = false;

        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            // Navigate back to the previous page
            await Navigation.PopAsync();
        }
    }
}
