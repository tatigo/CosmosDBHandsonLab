using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.CosmosSQL.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ScreenName { get; set; }
		public bool Verified { get; set; }
		public int FollowersCount { get; set; }
		public int FriendsCount { get; set; }
		public string Lang { get; set; }
		public string ProfileImageUrl { get; set; }
	
	}
}
