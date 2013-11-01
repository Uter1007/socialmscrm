using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Twitterizer;
using uter.tuwienscrm.authservice.Repository;

namespace uter.tuwienscrm.authtwitter.Controllers
{
    public class AuthController : Controller
    {
        //
        // GET: /Auth/

        private string TWITTER_CONSUMER_KEY = @"";
        private string TWITTER_CONSUMER_SECRET_KEY = @"";

        public CrmServerConfig Config { get; set; }
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public AuthController()
        {
            Config = ConfigHelper.ReadConfig();
            TWITTER_CONSUMER_KEY = ConfigurationManager.AppSettings["twitter_consumer_key"];
            TWITTER_CONSUMER_SECRET_KEY = ConfigurationManager.AppSettings["twitter_consumer_secret_key"];
        }

        public ActionResult Logon(string oauth_token, string oauth_verifier, string ReturnUrl)
        {

            if (String.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = "http://crm2011rc.dev.local/UterTwitter";
            }

            //Log.Trace("Twitter Key: {0}, Twitter Secret Key: {1}", TWITTER_CONSUMER_KEY, TWITTER_CONSUMER_SECRET_KEY);

            if (string.IsNullOrEmpty(oauth_token) || string.IsNullOrEmpty(oauth_verifier))
            {
                UriBuilder builder = new UriBuilder(this.Request.Url);
                builder.Query = string.Concat(
                    builder.Query,
                    string.IsNullOrEmpty(builder.Query) ? string.Empty : "&",
                    "ReturnUrl=",
                    ReturnUrl);

                var token = Twitterizer.OAuthUtility.GetRequestToken(TWITTER_CONSUMER_KEY, TWITTER_CONSUMER_SECRET_KEY, builder.ToString());

                return Redirect(OAuthUtility.BuildAuthorizationUri(token.Token, true).ToString());
            }

            var tokens = OAuthUtility.GetAccessToken(
                TWITTER_CONSUMER_KEY,
                TWITTER_CONSUMER_SECRET_KEY,
                oauth_token,
                oauth_verifier);

            Config = ConfigHelper.ReadConfig();

            var service = CRMConnection.Service(Config, true);

            var response = FindTwitterConfig(tokens, service);

            Log.Trace("Entities found = {0}", response.Entities.Count());

            if (response != null && response.Entities != null && response.Entities.Count() > 0 ){

                UpdateTwitterConfig(tokens, response, service);

            }else{

                CreateTwitterConfig(tokens,service);

            }

            if (string.IsNullOrEmpty(ReturnUrl))
                return Redirect("/");
            else
                return Redirect(ReturnUrl);
        }

        private void CreateTwitterConfig(OAuthTokenResponse tokens, IOrganizationService service)
        {
            var entity = new Entity("cott_twitterconfig");
            entity.Attributes.Add("cott_accesstoken", tokens.Token);
            entity.Attributes.Add("cott_accesstokensecret", tokens.TokenSecret);
            entity.Attributes.Add("cott_name", tokens.ScreenName);
            entity.Attributes.Add("cott_twitteruserid", tokens.UserId.ToString());
            entity.Attributes.Add("cott_count", 200);


            service.Create(entity);
        }

        private static void UpdateTwitterConfig(OAuthTokenResponse tokens, EntityCollection response, IOrganizationService service)
        {
            var rentity = response.Entities[0];

            if (rentity.Contains("cott_accesstoken"))
            {

                rentity["cott_accesstoken"] = tokens.Token;

            }
            else
            {

                rentity.Attributes.Add("cott_accesstoken", tokens.Token);

            }

            if (rentity.Contains("cott_accesstokensecret"))
            {

                rentity["cott_accesstokensecret"] = tokens.TokenSecret;

            }
            else
            {

                rentity.Attributes.Add("cott_accesstokensecret", tokens.TokenSecret);

            }

            
            service.Update(rentity);
        }

        private static EntityCollection FindTwitterConfig(OAuthTokenResponse tokens, IOrganizationService service)
        {
            Log.Trace("Find Twitter Configs");
            var query = new QueryByAttribute("cott_twitterconfig") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("cott_name");
            query.Values.Add(tokens.UserId.ToString());
            var response = service.RetrieveMultiple(query);
            return response;
        }
    }
}
