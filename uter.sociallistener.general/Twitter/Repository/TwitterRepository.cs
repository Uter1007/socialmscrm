using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.CRM.Models;
using uter.sociallistener.general.Social;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.Xrm.Sdk;
using Twitterizer;
using NLog;

namespace uter.sociallistener.general.Repository
{
    /// <summary>
    /// Twitter Repository
    /// </summary>
    public class TwitterRepository : ISocialRepository
    {

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string AccessKey {get;set;}
        public string AccessSecret {get;set;}
        public string ConsumerKey{get;set;}
        public string ConsumerSecret{get;set;}
        public TwitterConfig Config { get; set; }

        public OAuthTokens Access_Token {get;set;}

        public TwitterRepository(string consumer_key, string consumer_secret, TwitterConfig config)
        {
            var access_token = new OAuthTokens();
            access_token.AccessToken = config.Accesstoken;
            access_token.AccessTokenSecret = config.Accesstokensecret;
            access_token.ConsumerKey = consumer_key;
            access_token.ConsumerSecret = consumer_secret;
            Access_Token = access_token;
            Config = config;
        }

        /// <summary>
        /// Retrieve Feeds from Twitter HomeTimeline 
        /// </summary>
        /// <param name="config">Authentication Details</param>
        /// <param name="options">Additional Options to filter the Retrieve Statement</param>
        /// <returns>List </returns>
        public IList<TwitterFeed> RetrieveFeeds()
        {

            var hometimeline = RetrieveHomeTimeLine(Access_Token,Config);

            return hometimeline;
        }

        public TwitterUserProfile GetUserDetails()
        {
            TwitterResponse<TwitterUser> resp = TwitterAccount.VerifyCredentials(Access_Token);

            return TwitterUserMapper.Map(resp.ResponseObject);
        }

        public IList<TwitterUserProfile> GetFriends()
        {
            Log.Debug("Call Get Friends");
            var friendsResponse = TwitterFriendship.FriendsIds(Access_Token);

            if (friendsResponse != null)
            {

                var allFriends = GetFriendShipUsers(Access_Token, friendsResponse);
                return allFriends;
            }

            return null;
        }

        public IList<TwitterUserProfile> GetFollowers()
        {
            Log.Debug("Call Get Followers");
            var followersResponse = TwitterFriendship.FollowersIds(Access_Token);

            if (followersResponse != null)
            {
                var allFollowers = GetFriendShipUsers(Access_Token, followersResponse);

                return allFollowers;
            }

            return null;

        }

        #region Twittermethods

        private static List<TwitterFeed> RetrieveHomeTimeLine(OAuthTokens access_token, TwitterConfig config)
        {

            int max_recs = 1;
            string lastid = "";

            if (config.MaxRecords != 0)
            {
                max_recs = config.MaxRecords;
            }

            if (!String.IsNullOrEmpty(config.LastStatusId))
            {
                lastid = config.LastStatusId;
            }


            var timelineoptions = new TimelineOptions();
            timelineoptions.Count = (int)max_recs;
            timelineoptions.Page = 0;
            var tweetList = new List<TwitterFeed>();

            if (!String.IsNullOrEmpty(lastid))
            {
                timelineoptions.SinceStatusId = Decimal.Parse(lastid);
            }

            var getTimeline = TwitterTimeline.HomeTimeline(access_token, timelineoptions);

            if (getTimeline != null)
            {

                Log.Debug("TimeLine was not null");

                var tweets = getTimeline.ResponseObject;

                foreach (var tweet in tweets)
                {
                    tweetList.Add(TwitterFeedMapper.Map(tweet));
                }

                //tweetList.AddRange(tweets);

                //Check if other Tweets are available
                while (tweets.Count() != 0)
                {
                    timelineoptions.Page++;
                    getTimeline = TwitterTimeline.HomeTimeline(access_token, timelineoptions);
                    tweets = getTimeline.ResponseObject;
                    foreach (var tweet in tweets)
                    {
                        tweetList.Add(TwitterFeedMapper.Map(tweet));
                    }
                }

                var newList = tweetList.OrderBy(x => x.CreatedDate).ToList();

                return newList;
            }

            return null;
        }

        private static List<TwitterUserProfile> GetFriendShipUsers(OAuthTokens access_token, TwitterResponse<UserIdCollection> followersResponse)
        {
            Log.Debug("Call GetFriendShipUsers");
            Log.Debug("Access Token = {0}",access_token);

            TwitterResponse<TwitterUserCollection> userLookupResponse = null;
            var allusers = new List<TwitterUserProfile>();
            LookupUsersOptions lookupOptions = new LookupUsersOptions();

            if (followersResponse != null && followersResponse.ResponseObject != null){

                Log.Debug("Follower Response is not null");

                if (followersResponse.ResponseObject.Count > 0)
                {
                    for (int index = 0; index < followersResponse.ResponseObject.Count; index++)
                    {

                        if (index != 0 && index % 100 == 0)
                        {
                            userLookupResponse = TwitterUser.Lookup(access_token, lookupOptions);

                            if (userLookupResponse != null && userLookupResponse.ResponseObject != null)
                            {

                                for (int y = 0; y < userLookupResponse.ResponseObject.Count(); y++)
                                {
                                    allusers.Add(TwitterUserMapper.Map(userLookupResponse.ResponseObject[y]));
                                }
                            }

                            lookupOptions.UserIds.Clear();
                        }

                        lookupOptions.UserIds.Add(followersResponse.ResponseObject[index]);
                    }


                    userLookupResponse = TwitterUser.Lookup(access_token, lookupOptions);

                    for (int y = 0; y < userLookupResponse.ResponseObject.Count(); y++)
                    {
                        allusers.Add(TwitterUserMapper.Map(userLookupResponse.ResponseObject[y]));
                    }
                }
            }

            return allusers;
        }

        #endregion

        
    }
}
