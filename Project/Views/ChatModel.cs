using SQLite;
using System;

namespace Project.Models
{
    public class ChatMessage
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ClientName { get; set; }
        public string FreelancerName { get; set; }
        public string FreelancerJobTitle { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsSend { get; set; } = false;
    }
}
