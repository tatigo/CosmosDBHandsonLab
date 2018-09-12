using Azure.CosmosSQL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azure.CosmosSQL
{
    public static class TweetManager
    {
        public static List<Tweet> BullkCreateTweets()
        {
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(@"../../../TweetsData/Tweets.txt");

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

            User user = new User()
            {
                id = r.Next(),
                name = "CosmosDev",
                screenName = "CosmosDev",
                verified = true,
                followersCount = 10000,
                friendsCount = 500,
                lang = "en",
                profileImageUrl = "https://pbs.twimg.com/"
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
