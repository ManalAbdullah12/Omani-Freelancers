using Project.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreelancerChatList : ContentPage
    {
        private int _userId;
        private string _ClientName;
        public ObservableCollection<ClientListModel> _messages { get; set; }

        private string _dbPath;
        public FreelancerChatList(int userId, string dbPath, string CName)
        {
            InitializeComponent();
            _userId = userId;
            _dbPath = dbPath;
            _ClientName = CName;
            _messages = new ObservableCollection<ClientListModel>();
            BindingContext = this;

            // Load messages from database
            LoadMessages();

        }

        private void LoadMessages()
        {
            // Retrieve chat history from the database
            var chatService = new ChatService(_dbPath);
            var messages = new List<Models.ChatMessage>();
            messages = chatService.GetChatHistoryForFreelancerOnly(_userId);
            // Add retrieved messages to the chat messages collection
            var distinctFreelancers = messages.Select(msg => new { msg.ClientName, msg.SenderId, msg.FreelancerJobTitle }).Distinct().ToList();
            foreach (var message in distinctFreelancers)
            {
                _messages.Add(new ClientListModel { Id = message.SenderId, Name = message.ClientName, Job = message.FreelancerJobTitle });
                //_messages.Add(new ClientMessage { Text = message.Message, Sender = message.SenderId });
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            await Task.Delay(500);
            var frame = sender as Button;
            if (frame != null)
            {
                var selectedItem = frame.BindingContext as ClientListModel;
                if (selectedItem != null)
                {
                    await Navigation.PushAsync(new FreelancerChat(_userId,Convert.ToInt32(selectedItem.Id), _dbPath,  selectedItem.Name,_ClientName, selectedItem.Job));

                }
            }
            Isbusy.IsVisible = false;
        }



        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Isbusy.IsVisible = true;
            var frame = sender as Button;
            if (frame != null)
            {
                var selectedItem = frame.BindingContext as ClientListModel;
                if (selectedItem != null)
                {
                    var chatService = new ChatService(_dbPath);
                    var result = chatService.GetChatHistoryForUser(Convert.ToInt32(selectedItem.Id), _userId);
                    foreach (var chat in result)
                    {
                        chatService.DeleteMessagesById(chat);
                    }
                    _messages.Clear();
                    var messages = new List<Models.ChatMessage>();
                    messages = chatService.GetChatHistoryForFreelancerOnly(_userId);
                    // Add retrieved messages to the chat messages collection
                    var distinctFreelancers = messages.Select(msg => new { msg.ClientName, msg.SenderId, msg.FreelancerJobTitle }).Distinct().ToList();
                    foreach (var message in distinctFreelancers)
                    {
                        _messages.Add(new ClientListModel { Id = message.SenderId, Name = message.ClientName, Job = message.FreelancerJobTitle });
                        //_messages.Add(new ClientMessage { Text = message.Message, Sender = message.SenderId });
                    }
                }
            }
            Isbusy.IsVisible = false;
        }
    }
}