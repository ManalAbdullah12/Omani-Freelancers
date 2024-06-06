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
	public partial class JobHistory : ContentPage
	{
        private int _userid;
        public ObservableCollection<Jobs> Items { get; set; }
        public ICommand SubmitJobCommand { get; }
        public ICommand RejectJobCommand { get; }
        public JobHistory (int userData)
		{
			InitializeComponent ();
            _userid= userData;
            SubmitJobCommand = new Command<Jobs>(SubmitJob);
            RejectJobCommand = new Command<Jobs>(RejectJob);
            Items = new ObservableCollection<Jobs>();
            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            UserRepository userRepository = new UserRepository(dbpath);
            var posts = await userRepository.GetFreelancerJobs(_userid);
            Items.Clear();
            foreach (var post in posts)
            {
                Items.Add(post);
            }
        }
        private async void SubmitJob(Jobs jobs)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            if (jobs != null)
            {
                bool answer = await DisplayAlert("Accept Job Confirmation", "Are you sure you want to accept this job?", "Yes", "No");
                if (answer)
                {
                    var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                    UserRepository userRepository = new UserRepository(dbpath);
                    jobs.IsAccepted = true;
                    await userRepository.UpdateJob(jobs);
                    var posts = await userRepository.GetFreelancerJobs(_userid);
                    Items.Clear();
                    foreach (var post in posts)
                    {
                        Items.Add(post);
                    }
                }
            }
            IsBusy.IsVisible = false;
        }

        private async void RejectJob(Jobs jobs)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            if (jobs != null)
            {
                bool answer = await DisplayAlert("Accept Job Confirmation", "Are you sure you want to accept this job?", "Yes", "No");
                if (answer)
                {
                    var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
                    UserRepository userRepository = new UserRepository(dbpath);
                    await userRepository.DeleteJob(jobs);
                    var posts = await userRepository.GetFreelancerJobs(_userid);
                    Items.Clear();
                    foreach (var post in posts)
                    {
                        Items.Add(post);
                    }
                }
            }
            IsBusy.IsVisible=false;
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
                    await Navigation.PushAsync(new FreelancerJobHistoryDetails(selectedItem));

                }
            }
            IsBusy.IsVisible = false;
        }
    }
}