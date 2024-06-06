using SQLite;
using System;

namespace Project.Tables
{
    public class RegUserTable
    {
        [PrimaryKey, AutoIncrement] // Add AutoIncrement attribute
        public int Id { get; set; } // Change the data type to int
        public Guid UserID { get; set; }
        public double IdCardNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePic { get; set; } = string.Empty;
        public string IdCardPic { get; set; } = string.Empty;
        public string Name_ { get; set; } // Property for Name
        public string LastName { get; set; } // Property for Last Name
        public bool IsClient { get; set; } // Property for identifying if the user is a client
        public string FreelancerType { get; set; } // Property for storing freelancer type
        public int FollowersCount { get; set; } // Property for storing followers count
        public int AccountStatus { get; set; } = 0;

        // Constructor
        public RegUserTable()
        {
            UserID = Guid.NewGuid();
        }
    }
}
