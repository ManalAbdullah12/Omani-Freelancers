using Project.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentList : ContentPage
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
        public bool __IsClient;
        public bool _IsClient
    {
            get { return __IsClient; }
            set
            {
            __IsClient = value;
                OnPropertyChanged("_IsClient");
            }
        }
        public ObservableCollection<Jobs> Items { get; set; }
        public PaymentList(RegUserTable userData)
        {
            InitializeComponent();
            _userData = userData;
            _IsClient = _userData.IsClient;
            Items = new ObservableCollection<Jobs>();
            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            UserRepository userRepository = new UserRepository(dbpath);
            List<Jobs> posts = new List<Jobs>();
            if(_IsClient)
                posts = await userRepository.GetClientPayment(_userData.Id);
            else
                posts = await userRepository.GetFreelancerPayment(_userData.Id);
            Items.Clear();
            foreach (var post in posts)
            {
                Items.Add(post);
            }
        }

    }
}