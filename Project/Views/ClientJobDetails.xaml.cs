using Project.Tables;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientJobDetails : ContentPage, INotifyPropertyChanged
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

        public ClientJobDetails(Jobs userData)
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
    }
}
