using Project.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreelancerCurrentJobDetails : ContentPage, INotifyPropertyChanged
    {
        private Jobs _userData;
        public Jobs UserData
        {
            get => _userData;
            set
            {
                _userData = value;
                OnPropertyChanged(nameof(UserData));
            }
        }

        public FreelancerCurrentJobDetails(Jobs userData)
        {
            InitializeComponent();
            UserData = userData;
            BindingContext = this;
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            UserRepository userRepository = new UserRepository(dbpath);
            UserData.IsJobFinished = true;
            await userRepository.UpdateJob(UserData);
            await Navigation.PopAsync();
            IsBusy.IsVisible = false;
        }
    }
}