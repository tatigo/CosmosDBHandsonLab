using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.CosmosSQL.Models
{
    public class Tweet
    {
        public string Id { get;  set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
        public List<HashTag> HashTags { get; set; }
        
    }
}
