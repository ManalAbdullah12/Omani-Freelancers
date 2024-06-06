using Project.Models;
using Project.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Project.Views
{
    public partial class FreelancerChat : ContentPage
    {
        private int _userId;
        private int _freelancerid;
        private string _freelancerName;
        private string _ClientName;
        private string _freelancerJob;
        private ObservableCollection<Models.ChatMessage> _messages;
        private string _dbPath;

        public FreelancerChat(int FreelancerId, int userId, string dbPath, string CName, string FName, string FJob)
        {
            InitializeComponent();
            _userId = userId;
            _freelancerid = FreelancerId;
            _dbPath = dbPath;
            _freelancerName = FName;
            _freelancerJob = FJob;
            _ClientName = CName;
            ClientName.Text = _ClientName;
            _messages = new ObservableCollection<Models.ChatMessage>();

            // Load messages from database
            LoadMessages();

            ChatListView.ItemsSource = _messages;
        }

        private void LoadMessages()
        {
            // Retrieve chat history from the database
            var chatService = new ChatService(_dbPath);
            var messages = new List<Models.ChatMessage>();
            messages = chatService.GetChatHistoryForUser(_userId, _freelancerid);
            // Add retrieved messages to the chat messages collection
            foreach (var message in messages)
            {
                _messages.Add(message);
                //_messages.Add(new ClientMessage { Text = message.Message, Sender = message.SenderId });
            }
        }

        private void SendButton_Clicked(object sender, EventArgs e)
        {
            var messageText = MessageEntry.Text;
            if (!string.IsNullOrEmpty(messageText))
            {
                var message = new Models.ChatMessage();
                message = new Models.ChatMessage { Message = messageText, IsSend = false, ReceiverId = _freelancerid.ToString(), SenderId = _userId.ToString(), ClientName = _ClientName, FreelancerName = _freelancerName, FreelancerJobTitle = _freelancerJob };

                _messages.Add(message);

                // Save message to database
                SaveMessage(message);

                MessageEntry.Text = string.Empty;
            }
        }

        private void SaveMessage(Models.ChatMessage message)
        {
            // Save message to the database
            var chatService = new ChatService(_dbPath);
            chatService.SendMessageToFreelancer(message);
        }

        private void ChatListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null; // Deselect the selected item
        }
    }
}
