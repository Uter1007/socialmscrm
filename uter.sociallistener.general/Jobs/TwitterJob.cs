using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using uter.sociallistener.general.CRM;
using uter.sociallistener.general.CRM.Connection;
using uter.sociallistener.general.CRM.Models;
using uter.sociallistener.general.CRM.Models.Mapping;
using uter.sociallistener.general.CRM.Repository;
using uter.sociallistener.general.Repository;
using uter.sociallistener.general.Twitter.Models;
using System.ServiceModel;
using uter.sociallistener.general.Twitter.Repository;
using System.Threading;

namespace uter.sociallistener.general.Jobs
{
    public class TwitterJob
    {

        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();
        private static object syncLock = new object();
       
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        public CrmConnection Connection { get; set; }

        public void Run()
        {
            try
            {
                var start = DateTime.Now;
                Log.Debug("Start Twitter Job");            
                
                var service = Connection.BuildConnection();
               
                var crmRep = new CrmRepository(service);
                Log.Trace("Connection Established");

                Log.Trace("Retrieve Configs");
                var configs = crmRep.RetrieveTwitterConfigs();
                Log.Debug("Configs retrieved: {0}", configs.Count);


                foreach (var config in configs)
                {
                    Log.Debug("Build Twitter Repository");
                    //Depracted old API 
                    #region depracted
                    //var twitterRep = CreateTwitterRepository(ConsumerKey, ConsumerSecret, config);

                    //Log.Debug("Handle Friends & Followers");
                    //HandlePeerManagement(crmRep, config, twitterRep);

                    //Log.Trace("Retrieve Twitter Feeds");
                    //var newtweets = twitterRep.RetrieveFeeds();

                    //if (newtweets != null)
                    //{
                    //    Log.Debug("Retrieved Twitter Feeds = {0}", newtweets.Count);

                    //    CreateTwitterFeedsMultiThreaded(crmRep, config, newtweets);
                    //}
                    //else
                    //{
                    //    Log.Debug("No Feeds can't be found");
                    //}

                    //UpdateConfig(crmRep, config, newtweets, twitterRep);
                    #endregion

                    var twitterRep = new NewTwitterRepository(ConsumerKey, ConsumerSecret, config);

                    Log.Debug("Retrieve current Twitter Settings");
                    var user = twitterRep.GetUserDetails();

                    if (user != null)
                    {
                        Log.Debug("Handle Friends & Followers");
                        HandlePeerManagement(crmRep, config, twitterRep, user);

                        Log.Debug("Retrieve Twitter Feeds");
                        var newtweets = twitterRep.GetTimeLine(config.LastStatusId);

                        if (newtweets != null)
                        {
                            Log.Debug("Retrieved Twitter Feeds = {0}", newtweets.Count);
                            CreateTwitterFeedsMultiThreaded(crmRep, config, newtweets);

                            UpdateConfig(crmRep, config, newtweets, user);
                        }

                        
                    }
                    else
                    {
                        Log.Debug("User was null !");
                    }
                }

                var end = DateTime.Now;

                Log.Debug("End Service - Total Time = {0}", end - start);

            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Log.ErrorException("CRM Exception", ex);
                if (ex.Detail != null && ex.Detail.Message != null)
                {
                    Log.Error("Message: {0}", ex.Detail.Message);
                }

                if (ex.Detail != null )
                {
                    Log.Error("ErrorCode: {0}", ex.Detail.ErrorCode);
                }

                if (ex.Detail != null && ex.Detail.ErrorDetails != null)
                {
                    Log.Error("ErrorDetails: {0}", ex.Detail.ErrorDetails);
                }

                if (ex.Detail != null && ex.Detail.TraceText != null)
                {
                    Log.Error("TraceText: {0}", ex.Detail.TraceText);
                }

                if (ex.Detail != null && ex.Detail.InnerFault != null)
                {
                    Log.Error("InnerFault: {0}", ex.Detail.InnerFault);
                }

                if (ex.Detail != null && ex.Detail.TraceText != null)
                {
                    Log.Error("Trace: {0}", ex.Detail.TraceText);
                }
            }

            catch (Exception ex)
            {
                Log.ErrorException("Exception occures", ex);


            }


        }

        private static void CreateTwitterFeedsMultiThreaded(CrmRepository crmRep, TwitterConfig config, IList<TwitterFeed> newtweets)
        {
            ThreadPool.SetMaxThreads(10, 10);
            var fileParentTask = Task.Factory.StartNew(() =>
            {
                foreach (var newtweet in newtweets)
                {
                    var task = new Task(() =>
                            CreateTwitterFeed(crmRep, config, newtweet), TaskCreationOptions.AttachedToParent
                    );

                    task.Start();
                }
            }, TaskCreationOptions.LongRunning);

            Task.WaitAll(fileParentTask);
        }

        //private static TwitterRepository CreateTwitterRepository(string consumerKey, string consumerSecret, TwitterConfig config)
        //{
        //    Log.Debug("Get Twitter Repository");
        //    var twitterRep = new TwitterRepository(consumerKey, consumerSecret, config);
        //    Log.Debug("Return Twitter Repository");
        //    return twitterRep;
        //}

        private static void HandlePeerManagement(CrmRepository crmRep, TwitterConfig config, NewTwitterRepository twitterRep, TwitterUserProfile user)
        {

            Log.Trace("Handle Peer Management");

            Log.Debug("Retrieve Friends");
            var allFriends = twitterRep.GetFriends(user.Id);
            if (allFriends != null)
            {
                Log.Debug("Retrieved Friends = {0}", allFriends.Count);
                crmRep.HandleFollowers(config.CRMID, allFriends);
            }
            else
            {
                Log.Debug("Friends was null");
            }

            Log.Debug("Retrieve Followers");
            var allFollowers = twitterRep.GetFollowers(user.Id);

            if (allFollowers != null)
            {
                Log.Debug("Retrieved Followers = {0}", allFollowers.Count);

                crmRep.HandleFriends(config.CRMID, allFollowers);
            }
            else
            {
                Log.Debug("Followers was null");
            }
        }

        private static void UpdateConfig(CrmRepository crmRep, TwitterConfig config, IList<TwitterFeed> newtweets, TwitterUserProfile user)
        {

            UpdateConfigDetails(config, user);

            if (newtweets != null && newtweets.Count() > 0)
            {
                var laststatus = newtweets.Last<TwitterFeed>();
                config.LastStatusId = laststatus.Id.ToString();

                var entity = CRMTwitterConfigMapper.Map(config);

                crmRep.Service.Update(entity);

                Log.Trace("Updated Config with Laststatus = {0}", config.LastStatusId);
            }
        }

        private static void CreateTwitterFeed(CrmRepository crmRep, TwitterConfig config, TwitterFeed newtweet)
        {
            //Mutex - create

            lock (syncLock)
            {
                Log.Trace("Create Twitter Feed");
                var crmtwitterfeed = crmRep.UpCreateTwitterFeed(newtweet, config);

                var entities = newtweet.Entities;

                Log.Trace("Create Twitter Feed Entities");

                foreach (TwitterSocialEntity entity in entities)
                {
                    if (entity is TwitterHashTag)
                    {
                        crmRep.UpCreateHashTag(crmtwitterfeed, entity, config.CRMID);
                    }

                    if (entity is TwitterUrl)
                    {
                        crmRep.UpCreateTwitterUrl(crmtwitterfeed, entity, config.CRMID);
                    }

                    if (entity is TwitterMention)
                    {
                        crmRep.UpCreateTwitterMention(crmtwitterfeed, entity, config.CRMID);
                    }
                }
            }
        }

        private static void UpdateConfigDetails(TwitterConfig config, TwitterUserProfile user)
        {
            config.Location = user.Location;
            config.Website = user.Website;
            config.PersonalName = user.Name;
            config.TwitterPic = user.ProfileImageLocation;

        }

    }
}
