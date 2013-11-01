using Facebook;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Facebook.Models.DAO;
using uter.sociallistener.general.Facebook.Models.Mapping;
using uter.sociallistener.general.Social;

namespace uter.sociallistener.general.Facebook.Repository
{
    public class FacebookRepository : ISocialRepository
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public FacebookConfig Config { get; set; }

        public FacebookClient FbApp { get; set; }

        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public FacebookRepository(string consumerKey, string consumerSecret, FacebookConfig config)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            Config = config;
            FbApp = new FacebookClient(config.Accesstoken);

        }

        public FacebookConfig RenewToken()
        {
            dynamic result = FbApp.Get("oauth/access_token",
                           new
                           {
                               client_id = ConsumerKey,
                               client_secret = ConsumerSecret,
                               grant_type = "fb_exchange_token",
                               fb_exchange_token = Config.Accesstoken
                           });

            Config.Accesstoken = result.access_token;

            var expiredate = DateTime.UtcNow.AddSeconds(Convert.ToDouble(result.expires));

            Config.ExpireDate = expiredate;

            return Config;
        }

        public HomeProfile GetHomeProfile()
        {
            var model = FbApp.Get("me") as JsonObject;
            var user = FacebookUserMapper.Map(model);
            Config.UserName = user.UserName;
            Config.FirstName = user.FirstName;
            Config.LastName = user.LastName;
            Config.Link = user.Link;

            return new HomeProfile() { FacebookConfig = Config, FacebookUser = user };

        }

        public List<FacebookUser> GetFacebookFriends()
        {

            Log.Debug("Create FBUser List");
            var list = new List<FacebookUser>();

            dynamic userreq = FbApp.Get("/me/friends") as JsonObject;

            var users = (JsonArray)userreq.data;

            Log.Debug("Retrieved Users = {0}",users.Count);

            foreach (dynamic user in users)
            {
                var cuser = (IDictionary<String, object>)user;
               
                Log.Debug("Get User Details");

                var cuserid = (string)cuser["id"];

                Log.Debug("Get Details for UserId = {0}",cuserid);

                var muser = GetFacebookUser(cuserid);

                list.Add(muser);
            }

            return list;
        }

        public FacebookUser GetFacebookUser(string userid)
        {


            Log.Debug("Get more Facebook Infos");

            dynamic fulluser = FbApp.Get("/" + userid) as JsonObject;
            dynamic fulluserpic = FbApp.Get("/" + userid + "?fields=picture") as JsonObject;

            Log.Debug("Map Facebook User");

            var muser = FacebookUserMapper.Map(fulluser, fulluserpic);
            return muser;
        }

        public List<FacebookFeed> GetHomeFeeds()
        {
            var posts = new List<FacebookFeed>();

            Log.Debug("Get initial Feeds from Home Timeline");

            dynamic wall = GetInitialFeeds();


            Log.Debug("Get PagingKey");
            JsonArray wallposts = (JsonArray)wall.data;

            string next = "";

            if (((IDictionary<String, object>)wall).ContainsKey("paging"))
            {
                Log.Debug("Set Next Paging Key");
                next = wall.paging.next;
            }
            foreach (var wallpost in wallposts)
            {
                Log.Debug("Map Wallpost");
                posts.Add(FacebookFeedMapper.Map((JsonObject)wallpost));
            }

            Log.Debug("Do Paging");
            while (!String.IsNullOrEmpty(next) && posts.Count() > 0)
            {
                Log.Debug("Get Next Feeds");
                GetNextFeeds(posts, ref wallposts, ref next);
            }

            Log.Debug("Order the post DESC CreatedTime");
            if (posts.Count() > 0)
            {
                posts.OrderByDescending(x => x.CreatedTime);
            }

            Log.Debug("Return Posts");
            return posts;
        }

        private void GetNextFeeds(List<FacebookFeed> posts, ref JsonArray wallposts, ref string next)
        {
            Log.Debug("Run Fetch for next Feeds");
            dynamic nextwallposts = (JsonObject)FbApp.Get(next);
            wallposts = (JsonArray)nextwallposts.data;
            foreach (var wallpost in wallposts)
            {
                posts.Add(FacebookFeedMapper.Map((JsonObject)wallpost));
            }

            if (((IDictionary<String, object>)nextwallposts).ContainsKey("paging"))
            {
                next = nextwallposts.paging.next;
            }
            else
            {
                next = "";
            }
        }

        private dynamic GetInitialFeeds()
        {
            dynamic wall;
            if (Config.LastStatusDateTime != DateTime.MinValue)
            {
                Log.Debug("Last StatusDateTime = {0}",Config.LastStatusDateTime);
                dynamic parameters = new ExpandoObject();
                parameters.since = DateTimeToUnixTimestamp(Config.LastStatusDateTime);

                wall = (JsonObject)FbApp.Get("me/home", parameters);
            }
            else
            {
                Log.Debug("No StatusDateTime was found - use without Parameters");
                wall = (JsonObject)FbApp.Get("me/home");
            }


            Log.Debug("Return Initial Feeds");
            return wall;
        }

        public FacebookFeed GetFeed(string feedid)
        {
            var relmodel = FbApp.Get("/" + feedid) as JsonObject;
            return FacebookFeedMapper.Map(relmodel);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
