using System;
using System.Collections.Generic;
using SQLite;
using Project.Models;
using Project.Tables;

namespace Project.Services
{
    public class ChatService
    {
        private readonly SQLiteConnection _database;

        public ChatService(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<ChatMessage>(); // Create table for storing chat messages
        }

        // Method to send a chat message
        public void SendMessage(ChatMessage message)
        {
            try
            {
                _database.Insert(message);
            }
            catch (SQLiteException ex)
            {
                // Handle SQLite exceptions
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw; // Rethrow the exception
            }
        }

        // Method to retrieve chat history for a user
        public List<ChatMessage> GetChatHistoryForUser(int clientid,int freelancerid)
        {
            try
            {
                return _database.Table<ChatMessage>().Where(u => u.SenderId.Equals(clientid) && u.ReceiverId.Equals(freelancerid)).OrderBy(u => u.Timestamp).ToList();


                // Retrieve chat messages where the sender or receiver ID matches the given user ID
                // return _database.Query<ChatMessage>("SELECT * FROM ChatMessage WHERE SenderId = ? OR ReceiverId = ? ORDER BY Timestamp", freelancerid, clientid);
            }
            catch (SQLiteException ex)
            {
                // Handle SQLite exceptions
                Console.WriteLine($"Error retrieving chat history: {ex.Message}");
                throw; // Rethrow the exception
            }
        }

        public List<ChatMessage> GetChatHistoryForUserOnly(int senderId)
        {
            try
            {
                return _database.Table<ChatMessage>().Where(u => u.SenderId.Equals(senderId) ).OrderBy(u => u.Timestamp).ToList();

                // Retrieve chat messages where the sender ID matches the given user ID
                //   return _database.Query<ChatMessage>("SELECT * FROM ChatMessage WHERE SenderId = ? ORDER BY Timestamp", senderId);
            }
            catch (SQLiteException ex)
            {
                // Handle SQLite exceptions
                Console.WriteLine($"Error retrieving chat history: {ex.Message}");
                throw; // Rethrow the exception
            }
        }

        public List<ChatMessage> GetChatHistoryForFreelancerOnly(int ReceiverId)
        {
            try
            {
                return _database.Table<ChatMessage>().Where(u => u.ReceiverId.Equals(ReceiverId)).OrderBy(u => u.Timestamp).ToList();

                // Retrieve chat messages where the sender ID matches the given user ID
                // return _database.Query<ChatMessage>("SELECT * FROM ChatMessage WHERE ReceiverId = ? ORDER BY Timestamp", ReceiverId);
            }
            catch (SQLiteException ex)
            {
                // Handle SQLite exceptions
                Console.WriteLine($"Error retrieving chat history: {ex.Message}");
                throw; // Rethrow the exception
            }
        }


        // Method to send a message from freelancer to client
        public void SendMessageToClient(int clientId, int freelancerId, string messageText)
        {
            try
            {
                // Create a new chat message
                var message = new ChatMessage
                {
                    SenderId = freelancerId.ToString(),
                    ReceiverId = clientId.ToString(),
                    Message = messageText,
                    Timestamp = DateTime.Now,
                    IsSend = true
                };

                // Save the message to the database
                SendMessage(message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error sending message to client: {ex.Message}");
                throw; // Rethrow the exception
            }
        }

        // Method to send a message from client to freelancer
        public void SendMessageToFreelancer(Models.ChatMessage chatMessage)
        {
            try
            {
                SendMessage(chatMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error sending message to freelancer: {ex.Message}");
                throw; // Rethrow the exception
            }
        }

        public void DeleteMessagesById(ChatMessage chatMessage)
        {
            try
            {
                _database.Delete(chatMessage);
            }
            catch (SQLiteException ex)
            {
            }
        }

    }
}
