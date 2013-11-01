using LinqToTwitter;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.CRM.Models;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;

namespace uter.sociallistener.general.Twitter.Repository
{
    public class NewTwitterRepository
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public TwitterContext Context { get; set; }

        public NewTwitterRepository(string consumer_key, string consumer_secret, TwitterConfig config)
        {

            Log.Debug("Build Repository for new Twitter API");

            var auth = new SingleUserAuthorizer()
            {
                Credentials = new SingleUserInMemoryCredentials
                {
                    ConsumerKey = consumer_key,
                    ConsumerSecret = consumer_secret,
                    TwitterAccessToken = config.Accesstoken,
                    TwitterAccessTokenSecret = config.Accesstokensecret
                }
            };

            Log.Debug("Build Context");
            Context = new TwitterContext(auth);
            Log.Debug("TwitterContext finished");
        }

        public TwitterUserProfile GetUserDetails()
        {
            try{
                var account =
                    (from acct in Context.Account
                    where acct.Type == AccountType.VerifyCredentials
                    select acct.User).SingleOrDefault();

                if (account != null){
                    return TwitterUserMapper.Map(account);
                }

                return null;
            }
            catch (TwitterQueryException tqe)
            {
                Log.WarnException("Exception occured while Twitter Query: ", tqe);
                //if (tqe.Response != null)
                //{
                //    Log.Warn("Request = {0}", tqe.Response.Request);
                //    Log.Warn("Response = {0}",tqe.Response.Error);
                //}
                return null;
            }
        }

        public static List<List<string>> Split(List<string> source, int count)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / count)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public IList<TwitterUserProfile> GetFriends(decimal id)
        {
            try
            {
                var friendList = GetFriendIDs(id);

                if (friendList != null)
                {
                    Log.Debug("Count = {0}", friendList.IDs.Count);
                }

                return LookupUserIds(friendList);
            }
            catch (TwitterQueryException tqe)
            {
                Log.WarnException("Exception occured while Twitter Query: ", tqe);
                //if (tqe.Response != null)
                //{
                //    Log.Warn("Request = {0}", tqe.Response.Request);
                //    Log.Warn("Response = {0}", tqe.Response.Error);
                //}

                return null;
            }
        }

        public IList<TwitterFeed> GetTimeLine(string lastid)
        {

            Log.Debug("Call GetTimeLine with LastID = {0}",lastid);

            try{
                var limit = 0;
                ulong maxId;
                var statuslist = new List<Status>();

                var tweets = InitializeTimeLineRequest(lastid);

                if (tweets.Count > 0)
                {
                    statuslist.AddRange(tweets);

                    maxId = tweets.Min(status => ulong.Parse(status.StatusID)) - 1;

                    Log.Debug("MaxID = {0}", maxId);

                    do
                    {
                        limit++;
                        tweets =
                            (from tweet in Context.Status
                             where tweet.Type == StatusType.Home &&
                             tweet.SinceID == ulong.Parse(lastid) &&
                             tweet.Count == 200 &&
                             tweet.MaxID == maxId
                             select tweet).ToList();

                        Log.Debug("Tweets Count = {0}", tweets.Count);

                        if (tweets.Count > 0)
                        {
                            maxId = tweets.Min(status => ulong.Parse(status.StatusID)) - 1;

                            statuslist.AddRange(tweets);
                        }


                    } while (tweets.Count != 0 && statuslist.Count < 800);

                    var timeline = new List<TwitterFeed>();

                    Log.Debug("Gathered Tweets = {0}", statuslist.Count);

                    foreach (var tweet in statuslist)
                    {
                        var feed = TwitterFeedMapper.Map(tweet);
                        if (feed != null)
                        {
                            timeline.Add(feed);
                        }
                    }

                    var newList = timeline.OrderBy(x => x.CreatedDate).ToList();

                    return newList;
                }

                return null;
            }
            catch (TwitterQueryException tqe)
            {
                Log.WarnException("Exception occured while Twitter Query: ", tqe);
                //if (tqe.Response != null)
                //{
                //    Log.Warn("Request = {0}", tqe.Response.Request);
                //    Log.Warn("Response = {0}", tqe.Response.Error);
                //}
                return null;
            }
        }

        public List<Status> InitializeTimeLineRequest(string lastid)
        {

            Log.Debug("InitializeTimeLine");

            if (String.IsNullOrEmpty(lastid))
            {
                Log.Debug("Last ID was Empty");

                var ret = (from tweet in Context.Status
                where tweet.Type == StatusType.Home &&
                tweet.Count == 200
                select tweet).ToList();

                Log.Debug("Tweets Count = {0}", ret.Count);

                return ret;
            }
            else
            {

                Log.Debug("LastID was not empty");

                var ret = (from tweet in Context.Status
                     where tweet.Type == StatusType.Home &&
                     tweet.SinceID == ulong.Parse(lastid) &&
                     tweet.Count == 200
                     select tweet).ToList();

                Log.Debug("Tweets Count = {0}", ret.Count);

                return ret;
            }
        }

        public IList<TwitterUserProfile> GetFollowers(decimal id)
        {
            try
            {
                var followersList = GetFollowerIDs(id);
                if (followersList != null)
                {
                    Log.Debug("Count = {0}",followersList.IDs.Count);
                }
                return LookupUserIds(followersList);

            }
            catch (TwitterQueryException tqe)
            {
                Log.WarnException("Exception occured while Twitter Query: ", tqe);
                //if (tqe.Response != null)
                //{
                //    Log.Warn("Request = {0}", tqe.Response.Request);
                //    Log.Warn("Response = {0}",tqe.Response.Error);
                //}
                return null;
            }
        }

        private IList<TwitterUserProfile> LookupUserIds(SocialGraph friendList)
        {
            Log.Debug("Lookup Friends / Followers");

            var splittedidlists = Split(friendList.IDs, 90);

            var retfriends = new List<TwitterUserProfile>();

            foreach (var splittedlist in splittedidlists)
            {
                var joinedstrings = String.Join(",", splittedlist.ToArray());
                if (joinedstrings != null && joinedstrings.Length > 0)
                {
                    var users =
                    (from user in Context.User
                     where user.Type == UserType.Lookup &&
                           user.UserID == joinedstrings
                     select user)
                     .ToList();

                    Log.Debug("Retrieved Lookups = {0}", users.Count);

                    foreach (var user in users)
                    {
                        retfriends.Add(TwitterUserMapper.Map(user));
                    }
                }
                else
                {
                    Log.Warn("No JoinedStrings");
                }

            }

            return retfriends;
        }

        private SocialGraph GetFriendIDs(decimal id)
        {
            var friendList =
                (from friend in Context.SocialGraph
                 where friend.Type == SocialGraphType.Friends
                 && friend.UserID == id
                 select friend).SingleOrDefault();
            return friendList;
        }

        private SocialGraph GetFollowerIDs(decimal id)
        {
            var followerList =
                 (from follower in Context.SocialGraph
                  where follower.Type == SocialGraphType.Followers &&
                        follower.UserID == id
                  select follower)
                  .SingleOrDefault();

            return followerList;
        }

    }
}
