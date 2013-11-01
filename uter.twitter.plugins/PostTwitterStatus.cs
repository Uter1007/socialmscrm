using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using LinqToTwitter;
using System.Xml;


namespace uter.twitter.plugins
{
    public class PostTwitterStatus : IPlugin
    {

        private string ConsumerKey { get; set; }
        private string ConsumerSecretKey { get; set; }

        public PostTwitterStatus(string unsecureConfig, string secureConfig)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(unsecureConfig);

            ConsumerKey = PluginConfiguration.GetConfigDataString(doc, "ConsumerKey");
            ConsumerSecretKey = PluginConfiguration.GetConfigDataString(doc, "ConsumerSecretKey");
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //// Get a reference to the organization service.
            IOrganizationServiceFactory factory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            // Get a reference to the tracing service.
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
                {

                    // Obtain the target entity from the input parmameters.
                    Entity entity = (Entity)context.InputParameters["Target"];
                    tracingService.Trace("Check Properties");

                    if (entity.Contains("cott_createdbyconfig") && entity.Contains("subject") && entity.Contains("cott_direction"))
                    {
                        var configref = (EntityReference)entity["cott_createdbyconfig"];
                        var direction = (OptionSetValue)entity["cott_direction"];
                        var subject = (string)entity["subject"];

                        Status tweetResponse = null;

                        tracingService.Trace("Check Direction");

                        if (direction.Value == 455920001)
                        {
                            tracingService.Trace("Get Config");
                            var config = service.Retrieve(configref.LogicalName, configref.Id, new ColumnSet("cott_accesstoken", "cott_accesstokensecret"));

                            tracingService.Trace("Generate Twitter Context");
                            var tokens = CreateAccessToken((string)config["cott_accesstoken"], (string)config["cott_accesstokensecret"]);

                            if (entity.Contains("cott_retweeted") && (bool)entity["cott_retweeted"])
                            {
                                tracingService.Trace("Do Retweet Logic");
                                if (entity.Contains("cott_retweetfeed"))
                                {
                                    var rtweet = (EntityReference)entity["cott_retweetfeed"];

                                    var rtweetdetail = service.Retrieve("cott_twitterfeed", rtweet.Id, new ColumnSet("cott_tweetid"));

                                    var rtweetid = (string)rtweetdetail["cott_tweetid"];

                                    tweetResponse = tokens.Retweet(rtweetid);
                                }
                            }else if (entity.Contains("cott_repliedtofeed")){

                                tracingService.Trace("Replied TO Logic");

                                var rtweet = (EntityReference)entity["cott_repliedtofeed"];

                                var rtweetdetail = service.Retrieve("cott_twitterfeed", rtweet.Id, new ColumnSet("cott_tweetid"));

                                var rtweetid = (string)rtweetdetail["cott_tweetid"];

                                tweetResponse = tokens.UpdateStatus(subject, rtweetid);

                            }else
                            {
                                tracingService.Trace("Normal Send");
                                tweetResponse = tokens.UpdateStatus(subject);
                            }

                            tracingService.Trace("Set Attributes from REsponse");

                            tracingService.Trace("STATUS ID = {0}", tweetResponse.StatusID);
                            if (entity.Contains("cott_tweetid"))
                            {
                                entity["cott_tweetid"] = tweetResponse.StatusID;
                            }
                            else
                            {
                                entity.Attributes.Add("cott_tweetid", tweetResponse.StatusID);
                            }

                            tracingService.Trace("Date = {0}", tweetResponse.CreatedAt);
                            if (entity.Contains("cott_twitterdate"))
                            {
                                entity["cott_twitterdate"] = tweetResponse.CreatedAt;
                            }
                            else
                            {
                                entity.Attributes.Add("cott_twitterdate", tweetResponse.CreatedAt);
                            }

                            tracingService.Trace("USER NAME = {0}", tweetResponse.User.Name);
                            if (entity.Contains("cott_twittername"))
                            {
                                entity["cott_twittername"] = tweetResponse.User.Name;
                            }else{
                                entity.Attributes.Add("cott_twittername", tweetResponse.User.Name);
                            }

                            tracingService.Trace("USER ID = {0}", tweetResponse.User.Identifier.UserID);

                            if (entity.Contains("cott_twitterid"))
                            {
                                entity["cott_twitterid"] = tweetResponse.User.Identifier.UserID;
                            }
                            else
                            {
                                entity.Attributes.Add("cott_twitterid", tweetResponse.User.Identifier.UserID);
                            }

                            tracingService.Trace("Create / UPDATE User");
                            var xresultid = UpCreateUser(service, tweetResponse.User, config.Id, tracingService);

                            tracingService.Trace("Add REference");
                            if (entity.Contains("cott_twitteruser"))
                            {
                                entity["cott_twitteruser"] = new EntityReference("cott_twitteruser", (Guid)xresultid);
                            }
                            else
                            {
                                entity.Attributes.Add("cott_twitteruser", new EntityReference("cott_twitteruser", (Guid)xresultid));
                            }
                        }

                        
                    }

                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                tracingService.Trace(ex.ToString());
                tracingService.ToString();
            }
        }

        private TwitterContext CreateAccessToken(string accesskey, string access_secretkey)
        {
            var auth = new SingleUserAuthorizer()
            {
                Credentials = new SingleUserInMemoryCredentials
                {
                    ConsumerKey =  ConsumerKey,
                    ConsumerSecret =  ConsumerSecretKey,
                    TwitterAccessToken = accesskey,
                    TwitterAccessTokenSecret = access_secretkey
                }
            };

            return new TwitterContext(auth);
        }

        private static DataCollection<Entity> FindTwitterUser(IOrganizationService service, User twitteruser)
        {
            var query = new QueryByAttribute("cott_twitteruser") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("statecode");
            query.Attributes.Add("cott_userid");
            query.Values.Add(0);
            query.Values.Add(twitteruser.Identifier.UserID.ToString());
            var response = service.RetrieveMultiple(query);
            return response.Entities;
        }

        private static Guid? GetResult(DataCollection<Entity> results)
        {
            if (results.Count() > 0)
            {
                return results[0].Id;
            }

            return null;
        }

        private static Guid? UpCreateUser(IOrganizationService service, User user, Guid configId, ITracingService tracing)
        {
            tracing.Trace("Upcreate User");
            var xresults = FindTwitterUser(service, user);
            var xresultid = GetResult(xresults);

            if (xresultid == null)
            {
                tracing.Trace("Create User");
                var tuser = new Entity("cott_twitteruser");

                tuser.Attributes.Add("cott_name", user.Name);

                tuser.Attributes.Add("cott_userid", user.Identifier.UserID.ToString());
                tuser.Attributes.Add("cott_twitterpic", user.ProfileImageUrl);
                tuser.Attributes.Add("cott_createdbyconfig", new EntityReference("cott_twitterconfig", configId));
                xresultid = (Guid)service.Create(tuser);
            }
            else
            {
                tracing.Trace("Update User");
                // If User was just mentioned before there will be no Image
                var tuuserupdate = xresults[0];
                if (tuuserupdate.Contains("cott_twitterpic"))
                {
                    tuuserupdate["cott_twitterpic"] = user.ProfileImageUrl;
                }
                else
                {

                    tuuserupdate.Attributes.Add("cott_twitterpic", user.ProfileImageUrl);
                }
                service.Update(tuuserupdate);
            }
            return xresultid;
        }
    }
}
