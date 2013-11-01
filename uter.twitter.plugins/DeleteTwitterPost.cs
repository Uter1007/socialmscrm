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
    public class DeleteTwitterPost : IPlugin
    {

        private string ConsumerKey { get; set; }
        private string ConsumerSecretKey { get; set; }

        public DeleteTwitterPost(string unsecureConfig, string secureConfig)
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
                
                if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity)
                {

                    tracingService.Trace("Trace delete");
                    var preMessageImage = (Entity)context.PreEntityImages["PreImage"];

                    tracingService.Trace("Set Created by Config");
                    var configref = (EntityReference)preMessageImage["cott_createdbyconfig"];

                    var twitteruser = (EntityReference)preMessageImage["cott_twitteruser"];
                    var user = service.Retrieve(twitteruser.LogicalName, twitteruser.Id, new ColumnSet("cott_userid"));

                    tracingService.Trace("Retrieve Created by Config");
                    var config = service.Retrieve(configref.LogicalName, configref.Id, new ColumnSet("cott_accesstoken", "cott_accesstokensecret", "cott_twitteruserid"));

                    var userid = (string)user["cott_userid"];
                    var configid = (string)config["cott_twitteruserid"];

                    if (userid == configid)
                    {
                        tracingService.Trace("Get Auth Token");
                        var tokens = CreateAccessToken((string)config["cott_accesstoken"], (string)config["cott_accesstokensecret"]);

                        tracingService.Trace("Set Tweetid");
                        var tweetid = (string)preMessageImage["cott_tweetid"];

                        tracingService.Trace("Twitterizer Delete");
                        var response = tokens.DestroyStatus(tweetid);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                // Handle the exception.
            }
        }


        private TwitterContext CreateAccessToken(string accesskey, string access_secretkey)
        {
            var auth = new SingleUserAuthorizer()
            {
                Credentials = new SingleUserInMemoryCredentials
                {
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecretKey,
                    TwitterAccessToken = accesskey,
                    TwitterAccessTokenSecret = access_secretkey
                }
            };

            return new TwitterContext(auth);
        }
    }
}
