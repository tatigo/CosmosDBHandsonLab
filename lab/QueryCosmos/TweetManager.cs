using Azure.CosmosSQL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azure.CosmosSQL
{
    public static class TweetManager
    {
        private static readonly string[] users = new string[] {"AzureDev", "CosmosDev", "SQLGuru","MicrosoftDevelopers"};
        private static readonly int[] followers = new int[] { 50000, 20000, 10000, 70000 };
        private static readonly int[] friends = new int[] { 5000, 3000, 8000, 9000 };
        private static readonly string[] language = new string[] { "en", "fr", "de", "ge" };
        private static readonly string[] usersProfilesUrls = new string[] { "https://pbs001.twimg.com/", "https://pbs002.twimg.com/", "https://pbs003.twimg.com/", "https://pbs004.twimg.com/" };

        public static List<Tweet> CreateMultipleTweets()
        {
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(@"TweetsData/Tweets.txt");

                //Read the first line of text
                line = sr.ReadLine();

                var result = new List<Tweet>();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the lie to console window

                    var tweet = CreateTweet(line);
                    result.Add(tweet);
                    //Read the next line
                    line = sr.ReadLine();
                }

                //close the file
                sr.Close();

                return result;
            }
            catch (Exception e)
            {
                throw new Exception ("Exception: " + e.Message);
            }
        }

        public static Tweet CreateTweet(string message)
        {
            Tweet tweet = new Tweet
            { 
                CreatedAt = DateTime.Now,
                Text = message,
                User = CreateUser(),
                HashTags = CreateHashTags(message)
            };

            return tweet;
        }

        private static User CreateUser()
        {
            Random r = new Random();
            int userIndex = r.Next(0, users.Length);

            User user = new User()
            {
                Id = r.Next(),
                Name = users[userIndex].ToString(),
                ScreenName = users[userIndex].ToString(),
                Verified = true,
                FollowersCount = followers[userIndex],
                FriendsCount = friends[userIndex],
                Lang = language[userIndex],
                ProfileImageUrl = usersProfilesUrls[userIndex]
            };

            return user;
        }

        private static List<HashTag> CreateHashTags(string message)
        {
            string[] words = message.Split(' ');

            var result = new List<HashTag>();

            foreach(var word in words)
            {
                if(word.StartsWith("#"))
                {
                    result.Add(new HashTag
                    {
                        Text = word.ToLower(),

                    });
                }
            }

            return result;
        }
    }
}
