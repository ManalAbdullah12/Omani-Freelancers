using Project.Models;
using Project.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.SymbolStore;
using Xamarin.Forms;

namespace Project.Views
{
    public partial class ClientChat : ContentPage
    {
        private int _userId;
        private int _freelancerid;
        private string _freelancerName;
        private string _freelancerJob;
        private string _ClientName;
        private ObservableCollection<Models.ChatMessage> _messages;
        private string _dbPath;

        public ClientChat(int FreelancerId,int userId, string dbPath, string CName, string FName, string FJob)
        {
            InitializeComponent();
            _userId = userId;
            _freelancerid = FreelancerId;
            _dbPath = dbPath;
            _freelancerName = FName;
              _freelancerJob = FJob;
            FreelancerName.Text = FName;
            JobTitle.Text = _freelancerJob;
            _ClientName = CName;
            _messages = new ObservableCollection<Models.ChatMessage>();

            // Load messages from database
            LoadMessages();

            ChatListView.ItemsSource = _messages;
            
        }

        private void LoadMessages()
        {
            // Retrieve chat history from the database
            var chatService = new ChatService(_dbPath);
            var messages  = new List<Models.ChatMessage>(); 
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
                message = new Models.ChatMessage { Message = messageText, IsSend = true, ReceiverId = _freelancerid.ToString(), SenderId = _userId.ToString(), ClientName = _ClientName, FreelancerName = _freelancerName, FreelancerJobTitle = _freelancerJob };

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

    public class ClientMessage
    {
        public string Text { get; set; }
        public string Sender { get; set; }
    }
}
