using Xamarin.Forms;
using System.IO;
using Xamarin.Forms.Xaml;
using Project.Tables;
using SQLite;
using System;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgetPasswordPage : ContentPage
    {
        public ForgetPasswordPage()
        {
            InitializeComponent();
            LogoImage.Source = ImageSource.FromResource("Project.Images.newlogo.png");
        }

        private async void Button_Submit_Clicked(object sender, EventArgs e)
        {
            IsBusy.IsVisible = true;
            await Task.Delay(500);
            // Get the email entered by the user
            string email = EntryEmail.Text;

            // Get the database path
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");

            // Open database connection
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                // Check if the email exists in the database
                var user = conn.Table<RegUserTable>().FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    // If the email is not found, display an error message
                    await DisplayAlert("Error", "This email is not registered. Please sign up.", "OK");
                }
                else
                {
                    // Generate a random password
                    string newPassword = GenerateRandomPassword();

                    // Update the user's password in the database
                    user.Password = newPassword;
                    conn.Update(user);

                    // Send an email with the new password
                    bool emailSent = SendEmail(email, newPassword);

                    if (emailSent)
                    {
                        // If the email is sent successfully, display a success message
                        await DisplayAlert("Success", "A new password has been sent to your email.", "OK");

                        // Navigate back to the Login page
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        // If the email fails to send, display an error message
                        await DisplayAlert("Error", "Failed to send the email. Please try again later.", "OK");
                    }
                }
            }
            IsBusy.IsVisible = false;
        }

        private async void BackToLogin_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back to the Login page
        }

        private string GenerateRandomPassword()
        {
            // Generate a random password with 8 characters including letters and digits
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool SendEmail(string recipientEmail, string newPassword)
        {
            try
            {
                IsBusy.IsVisible = true;
                // Configure SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("mmlaj97050@gmail.com", "mmlaj97050939M"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                // Create email message
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("mmlaj97050@gmail.com"),
                    Subject = "Omani Freelancers App - New Password",
                    Body = $"Your new password is: {newPassword}"
                };

                mailMessage.To.Add(recipientEmail);

                // Send email
                smtpClient.Send(mailMessage);
                IsBusy.IsVisible = false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                IsBusy.IsVisible = false;
                return false;
            }
        }
    }
}
