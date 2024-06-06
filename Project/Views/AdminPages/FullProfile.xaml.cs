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

namespace Project.Views.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullProfile : ContentPage, INotifyPropertyChanged
    {

        private RegUserTable _UserData;
        public RegUserTable UserData
    {
            get { return _UserData; }
            set
            {
            _UserData = value;
                OnPropertyChanged("UserData");
            }
        }
        public FullProfile(RegUserTable regUserTable)
        {
            InitializeComponent();
            BindingContext = this;
            UserData = regUserTable;
            if (UserData != null && !string.IsNullOrWhiteSpace(UserData.ImagePic))
            {
                byte[] imageBytes = Convert.FromBase64String(UserData.ImagePic);
                ProfilePic.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }

            //if (UserData != null && !string.IsNullOrWhiteSpace(UserData.IdCardPic))
            //{
            //    byte[] imageBytes = Convert.FromBase64String(UserData.IdCardPic);
            //    IdCardPic.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            //}
        }
    }
}