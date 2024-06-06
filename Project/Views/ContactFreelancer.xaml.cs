using System;
using Xamarin.Forms;

namespace Project.Views
{
    public partial class ContactFreelancer : ContentPage
    {
        public ContactFreelancer()
        {
            InitializeComponent();

            // Attach the event handler for the Entry's Completed event
            messageEntry.Completed += OnMessageEntryCompleted;
        }

        private void OnMessageEntryCompleted(object sender, EventArgs e)
        {
            // Trigger the send action when the "Enter" key is pressed
            SendMessage_Clicked(sender, e);
        }

        private void SendMessage_Clicked(object sender, EventArgs e)
        {
            // Get the message from the Entry
            string message = messageEntry.Text;

            // Get the current timestamp in 12-hour format with AM/PM indicators
            string timestamp = DateTime.Now.ToString("hh:mm tt");

            // Create a new StackLayout with the user's message and timestamp
            StackLayout newMessage = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label
                    {
                        Text = "You",
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = message,
                        FontSize = 16,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    },
                    new Label
                    {
                        Text = timestamp,
                        FontSize = 12,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(10, 0, 0, 0),
                        TextColor = Color.Gray // Optional: Adjust the color of the timestamp
                    }
                }
            };

            // Add the new message to the chatMessages StackLayout
            chatMessages.Children.Add(newMessage);

            // Clear the Entry after sending the message
            messageEntry.Text = string.Empty;
        }
    }
}
