using Facebook;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.CRM.Connection;
using uter.sociallistener.general.CRM.Models.Mapping;
using uter.sociallistener.general.CRM.Repository;
using uter.sociallistener.general.Facebook.Models.DAO;
using uter.sociallistener.general.Facebook.Models.Mapping;
using uter.sociallistener.general.Facebook.Repository;

namespace uter.sociallistener.general.Jobs
{
    public class FacebookJob
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
                Log.Debug("Start Facebook Job");

                var service = Connection.BuildConnection();

                var crmRep = new CrmRepository(service);
                Log.Trace("Connection Established");

                Log.Trace("Retrieve Configs");
                var configs = crmRep.RetrieveFacebookConfigs();
                Log.Debug("Configs retrieved: {0}", configs.Count);

                foreach (var config in configs)
                {
                    Log.Debug("Create Facebook Repository");
                    var facebookRep = CreateFacebookRepository(ConsumerKey, ConsumerSecret, config);

                    if (config.NeedRefresh)
                    {
                        Log.Debug("Config needs new AccessToken");
                        var renewedconfig = facebookRep.RenewToken();
                        service.Update(CRMFacebookConfigMapper.Map(renewedconfig));
                    }
                    else
                    {
                        Log.Debug("Config Access Token is still valid");
                    }

                    Log.Debug("Refresh CRM / Config User");
                    var homeuser = facebookRep.GetHomeProfile();

                    Log.Debug("Refresh CRM / Config User");
                    var fbuserguid = crmRep.RetrieveFbUser(homeuser.FacebookUser.ID);

                    if (fbuserguid != null)
                    {
                        Log.Debug("Update CRM User");
                        homeuser.FacebookUser.CRMID = (Guid)fbuserguid;
                        service.Update(FacebookUserMapper.Map(homeuser.FacebookUser, config));
                    }
                    else
                    {
                        Log.Debug("Create CRM User");
                        service.Create(FacebookUserMapper.Map(homeuser.FacebookUser, config));
                    }

                    Log.Debug("Get Friends");

                    var friends = facebookRep.GetFacebookFriends();

                    Log.Debug("Retrieved Friends = {0}",friends.Count);

                    foreach (var friend in friends){

                        Log.Debug("Check if friends exist");
                        var fbfuserguid = crmRep.RetrieveFbUser(friend.ID);

                        if (fbfuserguid != null)
                        {
                            friend.CRMID = (Guid)fbfuserguid;
                            service.Update(FacebookUserMapper.Map(friend,config));
                        }
                        else
                        {
                            service.Create(FacebookUserMapper.Map(friend, config));
                        }
                    }


                    Log.Debug("Fetch Timeline from Home");
                    var posts = facebookRep.GetHomeFeeds();

                    Log.Debug("Retrieved Posts = {0}",posts.Count);


                    if (posts.Count > 0)
                    {
                        foreach (var post in posts)
                        {
                            Log.Debug("Do Action for Post = {0}", post.ID);

                            Guid? relid = null;

                            Log.Debug("Check if Post is a Like Story");
                            if (post.Story != null && !String.IsNullOrEmpty(post.Story) && post.Story.Contains("likes"))
                            {
                                Log.Debug("Post is a like - more details will be retrieved");
                                var trimed_statusid = post.ID.Split('_');

                                if (!String.IsNullOrEmpty((string)trimed_statusid[1]))
                                {
                                    try
                                    {

                                        Log.Debug("Get detailed Status for Trimed ID = {0}", (string)trimed_statusid[1]);
                                        var detailstatus = facebookRep.GetFeed((string)trimed_statusid[1]);

                                        Log.Debug("Check if FB Feed already exist");
                                        var fbfeeddetailguid = crmRep.RetrieveFbFeed(detailstatus.ID);

                                        Log.Debug("Check if FB User already exist");
                                        var fbuserdetailguid = crmRep.RetrieveFbUser(detailstatus.FromID);

                                        SetFBUser(service, facebookRep, detailstatus, fbuserdetailguid, config);

                                        SetFBRelatedFeed(service, post, detailstatus, fbfeeddetailguid, config);

                                    }
                                    catch (FacebookApiException apiex)
                                    {
                                        //ToDO: do something
                                    }

                                }
                            }
                            else
                            {
                                Log.Debug("Post has no Story or no Like is contained");
                            }

                            Log.Debug("Check if FB User already exist");
                            var fbpostuserguid = crmRep.RetrieveFbUser(post.FromID);
                            SetFBUser(service, facebookRep, post, fbpostuserguid, config);

                            Log.Debug("Check if FB Feed already exist");
                            var fbfeed = crmRep.RetrieveFbFeed(post.ID);
                            UpCreateFeed(service, config, post, fbfeed);

                            if (post.Comments != null && post.Comments.Count() > 0)
                            {
                                foreach (var comment in post.Comments)
                                {
                                    Log.Debug("Create / Update Comments");
                                    UpCreateFbComment(service, crmRep, config, facebookRep, comment);
                                }

                            }

                            if (post.Likes != null && post.Likes.Count() > 0)
                            {
                                foreach (var like in post.Likes)
                                {
                                    Log.Debug("Create / Update FB Like");
                                    UpCreateFbLike(service, crmRep, post, like, facebookRep, config);
                                }

                            }
                        }

                        SetLastDate(config, posts);
                        UpdateConfig(service, config);
                    }

                }



            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Log.ErrorException("CRM Exception", ex);

                if (ex.Detail != null)
                {
                    Log.Error("ErrorCode: {0}", ex.Detail.ErrorCode);
                }

                if (ex.Detail != null && ex.Detail.Message != null)
                {
                    Log.Error("Message: {0}", ex.Detail.Message);
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

        private static void UpdateConfig(Microsoft.Xrm.Sdk.IOrganizationService service, FacebookConfig config)
        {
            service.Update(CRMFacebookConfigMapper.Map(config));
        }

        private static void SetLastDate(FacebookConfig config, List<FacebookFeed> posts)
        {
            var lastpost = posts.First<FacebookFeed>().CreatedTime;

            config.LastStatusDateTime = lastpost;
        }

        private static void UpCreateFeed(Microsoft.Xrm.Sdk.IOrganizationService service, FacebookConfig config, FacebookFeed post, Guid? fbfeed)
        {
            if (fbfeed != null)
            {
                post.CRMID = (Guid)fbfeed;
                service.Update(FacebookFeedMapper.Map(post, config));
            }
            else
            {
                post.CRMID = service.Create(FacebookFeedMapper.Map(post, config));
            }
        }

        private static void UpCreateFbComment(Microsoft.Xrm.Sdk.IOrganizationService service, CrmRepository crmRep, FacebookConfig config, FacebookRepository facebookRep, FacebookComment comment)
        {
            Log.Debug("Upcreate Fb Comment");
            var commentguid = crmRep.RetrieveFbFeed(comment.ID);

            Log.Debug("Check User");            
            var fbusercommentguid = crmRep.RetrieveFbUser(comment.FromID);
            SetFBUser(service, facebookRep, comment, fbusercommentguid, config);


            if (commentguid != null)
            {
                Log.Debug("Comment Update"); 
                comment.CRMID = (Guid)commentguid;
                service.Update(FacebookFeedMapper.MapToComment(comment, config));
            }
            else
            {
                Log.Debug("Comment Create");
                comment.CRMID = service.Create(FacebookFeedMapper.MapToComment(comment, config));
            }
        }

        private static void UpCreateFbLike(Microsoft.Xrm.Sdk.IOrganizationService service, CrmRepository crmRep, FacebookFeed post, FacebookLike like, FacebookRepository facebookRep, FacebookConfig config)
        {
            var likeguid = crmRep.RetrieveFbLike(like.ID, post.CRMID);

            like.RelatedFeed = post.CRMID;
            
            Log.Debug("Refresh CRM / Config User - ID = {0}",like.ID);

            var fbuserguid = crmRep.RetrieveFbUser(like.ID);

            

            if (fbuserguid != null && fbuserguid != Guid.Empty)
            {
                Log.Debug("Retrieved FB User = {0}", fbuserguid);
                like.FacebookUser = (Guid)fbuserguid;
            }
            else
            {
                Log.Debug("No FBUser couldn't be found - Retrieve it from Facebook = {0}",like.ID);
                var fbuser = facebookRep.GetFacebookUser(like.ID);
                Log.Debug("Create FBUser in CRM");
                like.FacebookUser = service.Create(FacebookUserMapper.Map(fbuser, config));
            }
                        
            if (likeguid != null)
            {
                like.CRMID = (Guid)likeguid;
                service.Update(FacebookFeedMapper.MapToLike(like,config));
            }
            else
            {
                like.CRMID = service.Create(FacebookFeedMapper.MapToLike(like,config));
            }
        }

        private static void SetFBRelatedFeed(Microsoft.Xrm.Sdk.IOrganizationService service, FacebookFeed post, FacebookFeed detailstatus, Guid? fbfeedguid, FacebookConfig config)
        {
            if (fbfeedguid != null)
            {
                detailstatus.CRMID = (Guid)fbfeedguid;
                service.Update(FacebookFeedMapper.Map(detailstatus,config));
            }
            else
            {
                post.RelatedFeed = service.Create(FacebookFeedMapper.Map(detailstatus,config));
            }
        }

        private static void SetFBUser(Microsoft.Xrm.Sdk.IOrganizationService service, FacebookRepository facebookRep, FacebookFeed detailstatus, Guid? fbuserguid, FacebookConfig config)
        {
            if (fbuserguid != null)
            {
                detailstatus.FacebookUser = (Guid)fbuserguid;
            }
            else
            {
                var fbuser = facebookRep.GetFacebookUser(detailstatus.FromID);
                detailstatus.FacebookUser = service.Create(FacebookUserMapper.Map(fbuser,config));
            }
        }

        private static void SetFBUser(Microsoft.Xrm.Sdk.IOrganizationService service, FacebookRepository facebookRep, FacebookComment comment, Guid? fbuserguid, FacebookConfig config)
        {
            if (fbuserguid != null)
            {
                comment.FacebookUser = (Guid)fbuserguid;
            }
            else
            {
                var fbuser = facebookRep.GetFacebookUser(comment.FromID);
                comment.FacebookUser = service.Create(FacebookUserMapper.Map(fbuser,config));
            }
        }

        private static FacebookRepository CreateFacebookRepository(string consumerKey, string consumerSecret, FacebookConfig config)
        {

            var facebookRep = new FacebookRepository(consumerKey, consumerSecret, config);
            return facebookRep;
        }
    }
}
