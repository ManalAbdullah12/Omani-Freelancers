using Xamarin.Forms;

namespace Project.Views
{
    public partial class Profile : ContentPage
    {
        public Profile(string username)
        {
            InitializeComponent();

            // Fetch freelancer details from the database using the username
            FetchFreelancerDetails(username);
        }

        private void FetchFreelancerDetails(string username)
        {
            // Get freelancer details from the database based on the username
            // For now, let's assume we have the following dummy data
            string fullName = "Olivia Wilson";
            string freelancerType = "Photographer";

            // Update UI with freelancer details
            FullNameLabel.Text = fullName;
            UsernameLabel.Text = "@" + username;
            FreelancerTypeLabel.Text = freelancerType;
        }
    }
}
