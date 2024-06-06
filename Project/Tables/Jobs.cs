using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tables
{
    public class Jobs
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string FreelancerFullName { get; set; }
        public int FreelancerId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string StartingDate { get; set; }
        public string TimeLine { get; set; }
        public double Price { get; set; }
        public bool IsAccepted { get; set; } = false;
        public bool IsPayment { get; set; } = false;
        public bool IsJobFinished { get; set; } = false;
        public string PhoneNumber { get; set; }
        public string IDcardNumber { get; set; }
        public string JobCreatedDate { get; set; } = DateTime.Now.ToString();
    }
}
