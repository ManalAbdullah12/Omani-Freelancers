using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tables
{
    public class Posts
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int FreelancerId { get; set; }
        public bool IsClient { get; set; }
        public string PostImage { get; set; } = string.Empty;
        public string PostLink { get; set; } = string.Empty;
        public string PostDetails { get; set; }
        public bool IsLink { get; set; } = false;
    }
}
