using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.CosmosSQL.Models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string screenName { get; set; }
        public bool verified { get; set; }
        public int followersCount { get; set; }
        public int friendsCount { get; set; }
        public string lang { get; set; }
        public string profileImageUrl { get; set; }
	
    }
}
