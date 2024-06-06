using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loader : Grid
    {
        public Loader()
        {
            InitializeComponent();
            LogoImage.Source = ImageSource.FromResource("Project.Images.newlogo.png");
        }
    }
}
