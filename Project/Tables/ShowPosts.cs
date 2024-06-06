using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Project.Tables
{
    public class ShowPosts
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int FreelancerId { get; set; }
        public bool IsClient { get; set; }
        public string PostImage { get; set; } = string.Empty;
        public string PostLink { get; set; } = string.Empty;
        public string PostDetails { get; set; }
        public bool IsLink { get; set; } = false;
        public ImageSource ImageSource { get; set; }
    }
}
