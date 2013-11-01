using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uter.sociallistener.general.CRM.Models;
using uter.sociallistener.general.CRM.Models.Mapping;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NLog;
using Twitterizer;
using Twitterizer.Entities;
using uter.sociallistener.general.Facebook.Models.DAO;

namespace uter.sociallistener.general.CRM.Repository
{
    public class CrmRepository
    {

        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();
        
        private object twitter_syncHashLock = new object();
        private object twitter_syncMentionLock = new object();
        private object twitter_syncUrlLock = new object();
        private object twitter_syncUserLock = new object();

        public IOrganizationService Service { get; set; }
        public Dictionary<string, Guid> ConnectionRoles { get; set; }

        public CrmRepository(IOrganizationService service)
        {
            if (service != null)
            {
                Service = service;
                InitializeConnectionRoles();
                
            }
        }

        private void InitializeConnectionRoles()
        {
            var connectionroles = new List<string>();

            var dicconnectionroles = new Dictionary<string, Guid>();

            connectionroles.Add("Hashtags");
            connectionroles.Add("Hashtags by");
            connectionroles.Add("Mentioned by");
            connectionroles.Add("Mentions");
            connectionroles.Add("URLs");
            connectionroles.Add("URLs by");
            connectionroles.Add("Following");
            connectionroles.Add("Followed by");

            foreach (var key in connectionroles)
            {
                var connresults = FindConnectionRole(key);

                var resultid = GetResult(connresults);

                if (resultid != null)
                {
                    dicconnectionroles.Add(key, (Guid)resultid);
                }
            }

            ConnectionRoles = dicconnectionroles;
        }

        private Guid? GetResult(DataCollection<Entity> results)
        {
            if (results.Count() > 0)
            {
                return results[0].Id;
            }

            return null;
        }

        public void CreateConnection(Guid record1id, Guid record2id, string record1name, string record2name, Guid role1id, Guid role2id)
        {
            if (!ConnectionExists(record1id, record2id))
            {

                var newConnection = new Entity("connection");
                newConnection.Attributes.Add("record1id", new EntityReference(record1name, record1id));
                newConnection.Attributes.Add("record2id", new EntityReference(record2name, record2id));
                newConnection.Attributes.Add("record1roleid", new EntityReference("connectionrole", role1id));
                newConnection.Attributes.Add("record2roleid", new EntityReference("connectionrole", role2id));


                var _connectionId = Service.Create(newConnection);


            }
        }



        public bool ConnectionExists(Guid record1id, Guid record2id)
        {
            var query = new QueryExpression
            {
                EntityName = "connection",
                ColumnSet = new ColumnSet("connectionid"),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions = 
                            {
                                new ConditionExpression
                                {
                                    AttributeName = "record1id",
                                    Operator = ConditionOperator.Equal,
                                    Values = { record1id }
                                },
                                 new ConditionExpression
                                {
                                    AttributeName = "record2id",
                                    Operator = ConditionOperator.Equal,
                                    Values = { record2id  }
                                }
                            }
                }
            };

            return Service.RetrieveMultiple(query).Entities.Count != 0;


        }

        public List<TwitterConfig> RetrieveTwitterConfigs()
        {
            var list = new List<TwitterConfig>();
            var configs =  FindTwitterConfig();
            foreach (var config in configs)
            {
                list.Add(CRMTwitterConfigMapper.Map(config));
            }

            return list;
        }

        public List<FacebookConfig> RetrieveFacebookConfigs()
        {
            var list = new List<FacebookConfig>();
            var configs = FindFacebookConfig();
            foreach (var config in configs)
            {
                list.Add(CRMFacebookConfigMapper.Map(config));
            }

            return list;
        }

        private DataCollection<Entity> FindFacebookConfig()
        {
            var query = new QueryByAttribute("cott_facebookconfig") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Values.Add(0);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public Guid? RetrieveFbFeed(string feedId)
        {
            var results = FindFbFeed(feedId);

            var resultid = GetResult(results);

            return resultid;
        }

        public Guid? RetrieveFbUser(string facebookuserid)
        {
            var results = FindFacebookUser(facebookuserid);

            var resultid = GetResult(results);

            return resultid;
        }

        public Guid? RetrieveFbLike(string facebooklikeid, Guid relatedfeed)
        {
            var results = FindFbLike(facebooklikeid, relatedfeed);

            var resultid = GetResult(results);

            return resultid;
        }

        private DataCollection<Entity> FindFbFeed(string feedId)
        {
            var query = new QueryByAttribute("cott_facebookfeed") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("cott_facebook_statusid");
            //query.Attributes.Add("cott_direction");
            query.Values.Add(feedId);
            //query.Values.Add(455920000);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        private DataCollection<Entity> FindFbLike(string userId, Guid relatedfeed)
        {
            var query = new QueryByAttribute("cott_fblike") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("cott_facebook_userid");
            //query.Attributes.Add("cott_direction");
            query.Values.Add(userId);
            query.Attributes.Add("cott_relatedfeed");
            query.Values.Add(relatedfeed);
            //query.Values.Add(455920000);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        private DataCollection<Entity> FindFacebookUser(string facebookuserid)
        {
            var query = new QueryByAttribute("cott_facebookuser") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Attributes.Add("cott_userid");
            query.Values.Add(0);
            query.Values.Add(facebookuserid);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public DataCollection<Entity> FindTwitterConfig()
        {
            var query = new QueryByAttribute("cott_twitterconfig") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Values.Add(0);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public DataCollection<Entity> FindTweet(string tweetid)
        {
            var query = new QueryByAttribute("cott_twitterfeed") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("cott_tweetid");
            //query.Attributes.Add("cott_direction");
            query.Values.Add(tweetid);
            //query.Values.Add(455920000);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public DataCollection<Entity> FindTwitterHashTagEntity(string hashtag)
        {
            var query = new QueryByAttribute("cott_twitterhashtag") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Attributes.Add("cott_name");
            query.Values.Add(0);
            query.Values.Add(hashtag);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public DataCollection<Entity> FindConnectionRole(string connectionrole)
        {
            var query = new QueryByAttribute("connectionrole") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Attributes.Add("name");
            query.Values.Add(0);
            query.Values.Add(connectionrole);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public DataCollection<Entity> FindTwitterUrl(string url)
        {
            var query = new QueryByAttribute("cott_twitterurl") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Attributes.Add("cott_name");
            query.Values.Add(0);
            query.Values.Add(url);
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public DataCollection<Entity> FindMentionedTwitterUser(TwitterMention mentioneduser)
        {
            var query = new QueryByAttribute("cott_twitteruser") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Attributes.Add("cott_userid");
            query.Values.Add(0);
            query.Values.Add(mentioneduser.UserId.ToString());
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public DataCollection<Entity> FindTwitterUser(TwitterUserProfile user)
        {
            var query = new QueryByAttribute("cott_twitteruser") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Attributes.Add("cott_userid");
            query.Values.Add(0);
            query.Values.Add(user.Id.ToString());
            var response = Service.RetrieveMultiple(query);
            return response.Entities;
        }

        public Guid UpCreateTwitterFeed(TwitterFeed tweet, TwitterConfig config)
        {

            var xresults = FindTweet(tweet.Id.ToString());
            var tweetid = GetResult(xresults);
            if (tweetid == null)
            {                         
                var xresultid = UpCreateUser(tweet.User, config.CRMID);
                tweet.User.CRMID = (Guid)xresultid;
                var crmtweet = TwitterFeedMapper.Map(tweet, config);

                return Service.Create(crmtweet);

            }
            else
            {

                var xresultid = UpCreateUser(tweet.User, config.CRMID);
                tweet.User.CRMID = (Guid)xresultid;
                tweet.CRMID = (Guid)tweetid;
                var crmtweetupdate = TwitterFeedMapper.Map(tweet, config);

                ReopenTweet(crmtweetupdate);
                             
                Service.Update(crmtweetupdate);

                RecloseTweet(crmtweetupdate);

                return crmtweetupdate.Id;

            }
        }



        private void RecloseTweet(Entity crmtweetupdate)
        {
            //Reclose Tweet
            SetStateRequest ssr2 = new SetStateRequest();
            ssr2.EntityMoniker = crmtweetupdate.ToEntityReference();
            ssr2.State = new OptionSetValue(1);
            ssr2.Status = new OptionSetValue(2);

            SetStateResponse resp2 = (SetStateResponse)Service.Execute(ssr2);
        }

        private void ReopenTweet(Entity crmtweetupdate)
        {
            //Reopen Tweet
            SetStateRequest ssr = new SetStateRequest();
            ssr.EntityMoniker = crmtweetupdate.ToEntityReference();
            ssr.State = new OptionSetValue();
            ssr.Status = new OptionSetValue(1);

            SetStateResponse resp1 = (SetStateResponse)Service.Execute(ssr);
        }

        public Guid? UpCreateUser(TwitterUserProfile user, Guid configId)
        {

            Log.Trace("Try to Retrieve Twitter User with the following ScreenName: {0}",user.ScreenName);
            var xresults = FindTwitterUser(user);
            var xresultid = GetResult(xresults);

            if (xresultid == null)
            {
                Log.Trace("No User with the following ScreenName: {0} was found", user.ScreenName);
                var tuser = TwitterUserMapper.Map(user, configId);
                xresultid = (Guid)Service.Create(tuser);
                Log.Trace("Twitteruser with ID = {0} was created", xresultid);
            }
            else
            {
                Log.Trace("User with the following ScreenName: {0} was found", user.ScreenName);
                var tuser = TwitterUserMapper.Map(user, configId);
                tuser.Id = xresults[0].Id;
                Service.Update(tuser);
                Log.Trace("Twitteruser with ID = {0} was updated", tuser.Id);
            }
            return xresultid;
        }

        public Guid? UpCreateHashTag(Guid crmtwitterfeed, TwitterSocialEntity entity, Guid configId)
        {
            TwitterHashTag tagEntity = (TwitterHashTag)entity;

            var resultid = new Nullable<Guid>();

            lock (twitter_syncHashLock)
            {
                var results = FindTwitterHashTagEntity(tagEntity.Text);

                resultid = GetResult(results);

                if (resultid == null)
                {
                    var hashtag = TwitterHashTagMapper.Map(tagEntity, configId);
                    resultid = (Guid)Service.Create(hashtag);
                }

                var conrole2 = ConnectionRoles["Hashtags"];
                var conrole1 = ConnectionRoles["Hashtags by"];

                if (conrole1 != Guid.Empty && conrole2 != Guid.Empty)
                {
                    CreateConnection(crmtwitterfeed, (Guid)resultid, "cott_twitterfeed", "cott_twitterhashtag", conrole1, conrole2);
                }
            }
            
        
            return resultid;


        }

        public Guid? UpCreateTwitterMention(Guid crmtwitterfeed, TwitterSocialEntity entity, Guid configId)
        {
            TwitterMention mentionEntity = (TwitterMention)entity;
            var resultid = new Nullable<Guid>();

            lock (twitter_syncMentionLock)
            {
         
                var results = FindMentionedTwitterUser(mentionEntity);

                resultid = GetResult(results);

                if (resultid == null)
                {
                    var tuser = TwitterMentionMapper.Map(mentionEntity,configId);
                    resultid = (Guid)Service.Create(tuser);
                }

                var conrole2 = ConnectionRoles["Mentions"];
                var conrole1 = ConnectionRoles["Mentioned by"];

                if (conrole1 != Guid.Empty && conrole2 != Guid.Empty)
                {
                    CreateConnection(crmtwitterfeed, (Guid)resultid, "cott_twitterfeed", "cott_twitteruser", conrole1, conrole2);
                }
            }
           
           
            return resultid;

        }

        public Guid? UpCreateTwitterUrl(Guid crmtwitterfeed, TwitterSocialEntity entity, Guid configId)
        {
            TwitterUrl urlEntity = (TwitterUrl)entity;
            var resultid = new Nullable<Guid>();
            lock (twitter_syncUrlLock)
            {
                var results = FindTwitterUrl(urlEntity.Url);

                resultid = GetResult(results);

                if (resultid == null)
                {
                    var turl = TwitterUrlMapper.Map(urlEntity, configId);
                    resultid = (Guid)Service.Create(turl);
                }

                var conrole2 = ConnectionRoles["URLs"];
                var conrole1 = ConnectionRoles["URLs by"];

                if (conrole1 != Guid.Empty && conrole2 != Guid.Empty)
                {
                    CreateConnection(crmtwitterfeed, (Guid)resultid, "cott_twitterfeed", "cott_twitterurl", conrole1, conrole2);
                }
            }
            
            return resultid;
        }

        public void HandleFollowers(Guid configId, IList<TwitterUserProfile> allFollowers)
        {
            ThreadPool.SetMaxThreads(10, 10);
            var fileParentTask = Task.Factory.StartNew(() =>
                {
                    foreach (var user in allFollowers)
                    {
                        var task = new Task(() =>
                            HandleFollower(configId, user), TaskCreationOptions.AttachedToParent);
                        task.Start();
                    }

                }, TaskCreationOptions.LongRunning);

            Task.WaitAll(fileParentTask);
        }

        private void HandleFollower(Guid configId, TwitterUserProfile user)
        {
            lock (twitter_syncUserLock)
            {
                var xresultid = UpCreateUser(user, configId);
                var conrole2 = ConnectionRoles["Following"];
                var conrole1 = ConnectionRoles["Followed by"];

                if (conrole1 != Guid.Empty && conrole2 != Guid.Empty)
                {
                    CreateConnection(configId, (Guid)xresultid, "cott_twitterconfig", "cott_twitteruser", conrole1, conrole2);
                }
            }
            
        }

        public void HandleFriends(Guid configId, IList<TwitterUserProfile> allFriends)
        {
            ThreadPool.SetMaxThreads(10,10);
            var fileParentTask = Task.Factory.StartNew(() =>
            {
                foreach (var user in allFriends)
                {
                    var task = new Task(() =>
                        HandleFriend(configId, user), TaskCreationOptions.AttachedToParent);
                    task.Start();
                }

            }, TaskCreationOptions.LongRunning);

            Task.WaitAll(fileParentTask);
        }

        private void HandleFriend(Guid configId, TwitterUserProfile user)
        {
            lock (twitter_syncUserLock)
            {
                var xresultid = UpCreateUser(user, configId);
                var conrole2 = ConnectionRoles["Following"];
                var conrole1 = ConnectionRoles["Followed by"];

                if (conrole1 != Guid.Empty && conrole2 != Guid.Empty)
                {
                    CreateConnection(configId, (Guid)xresultid, "cott_twitterconfig", "cott_twitteruser", conrole2, conrole1);
                }
            }
            
        }
    }
}
