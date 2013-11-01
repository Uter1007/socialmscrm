using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using uter.tuwien.authservice.Repository;
using Facebook;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace uter.tuwienscrm.authservice.Controllers
{
    public class FacebookController : Controller
    {
        //
        // GET: /Facebook/

        public const string NOT_AUTHORIZED = "Not Authorized";
        public const string FACEBOOK_USER_DENIED = "user_denied";
        public const string FACEBOOK_ERROR_REASON = "error_reason";

        private string FACEBOOK_KEY { get; set; }
        private string FACEBOOK_SECRET_KEY { get; set; }

        public CrmServerConfig Config { get; set; }
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public string FACEBOOK_LOGIN_COMPLETE_URL {get;set;}
        public string FACEBOOK_SCOPE { get; set; }

        public string Home_ADDRESS { get; set; }

        public FacebookController()
        {
            Config = ConfigHelper.ReadConfig();
            FACEBOOK_KEY = ConfigurationManager.AppSettings["facebook_key"];
            FACEBOOK_SECRET_KEY = ConfigurationManager.AppSettings["facebook_secret_key"];

            FACEBOOK_LOGIN_COMPLETE_URL = ConfigurationManager.AppSettings["facebook_login_complete_url"];
            FACEBOOK_SCOPE = ConfigurationManager.AppSettings["facebook_scope"];
            Home_ADDRESS = ConfigurationManager.AppSettings["home_address"];
        }


        public ActionResult Logon(string returnUrl)
        {

            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Home_ADDRESS;
            }

            var url = string.Format(@"https://www.facebook.com/dialog/oauth/?client_id={0}&redirect_uri={1}&scope={2}&state={3}",
                                                           FACEBOOK_KEY,
                                                          FACEBOOK_LOGIN_COMPLETE_URL,
                                                           FACEBOOK_SCOPE,
                                                           returnUrl );

            return Redirect(url);

        
        }

        public ActionResult FacebookLoginComplete(string state, string code)
        {

            ////user denied permissions on Facebook. 
            //if (Request[Constants.FACEBOOK_ERROR_REASON] == Constants.FACEBOOK_USER_DENIED)
            //{
            //    //this is not implemented. For reference only.
            //    return RedirectToAction("StaticPage", "Default", new { name = "Privacy-Policy" });
            //}

            Log.Trace("Facebook Complete");

            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Error = "There was an error while loggin into Facebook. Please try again later.";
                return RedirectToAction("Logon");
            }


            var returnUrl = state;
            var datetoday = DateTime.Now;
            dynamic tokenresponse = GetFacebookAccessToken(code, returnUrl, FACEBOOK_LOGIN_COMPLETE_URL);
            var token = tokenresponse.access_token as string;

            var expiredate = DateTime.UtcNow.AddSeconds(Convert.ToDouble(tokenresponse.expires));

            dynamic fresponse = GetFacebookResponse("me", token);

            string uid = fresponse.id;

            if (!string.IsNullOrEmpty(uid))
            {

                var service = CRMConnection.Service(Config, true);

                var response = FindFacebookConfig(uid, service);

                Log.Trace("Entities found = {0}", response.Entities.Count());

                if (response != null && response.Entities != null && response.Entities.Count() > 0)
                {

                    UpdateFacebookConfig(fresponse, token, expiredate ,response, service);

                }
                else
                {

                    CreateFacebookConfig(fresponse, token, expiredate, service);

                }

                return Redirect(Home_ADDRESS);
            }
            else
            {
                return View("NotLoggedIn", model: returnUrl);
            }
        }

        public dynamic GetFacebookResponse(string actionUrl, string accessToken)
        {
            FacebookClient FbApp;
            if (string.IsNullOrEmpty(accessToken))
            {
                FbApp = new FacebookClient();
            }
            else
            {
                FbApp = new FacebookClient(accessToken);
            }
            return FbApp.Get(actionUrl) as JsonObject;
        }

        public dynamic GetFacebookAccessToken(string code, string returnUrl, string fbRedirectUri)
        {
            var f = new FacebookClient();
            dynamic result = f.Get("oauth/access_token", new
            {
                client_id = FACEBOOK_KEY,
                client_secret = FACEBOOK_SECRET_KEY,
                redirect_uri = fbRedirectUri,
                code = code,
                state = returnUrl
            });
            return result;
        }

        private static EntityCollection FindFacebookConfig(string userid, IOrganizationService service)
        {
            Log.Trace("Find Facebook Configs");
            var query = new QueryByAttribute("cott_facebookconfig") { ColumnSet = new ColumnSet(true) };
            query.Attributes.Add("cott_facebookuserid");
            query.Values.Add(userid);
            var response = service.RetrieveMultiple(query);
            return response;
        }

        private void CreateFacebookConfig(dynamic fconfig, string token, DateTime expieres , IOrganizationService service)
        {
            var entity = new Entity("cott_facebookconfig");
            entity.Attributes.Add("cott_accesstoken", token);
            entity.Attributes.Add("cott_name", fconfig.username);
            entity.Attributes.Add("cott_facebookuserid", fconfig.id);
            entity.Attributes.Add("cott_expieredate", expieres);
            entity.Attributes.Add("cott_email", fconfig.email);
            entity.Attributes.Add("cott_facebooklink", fconfig.link);

            entity.Attributes.Add("cott_firstname", fconfig.first_name);
            entity.Attributes.Add("cott_lastname", fconfig.last_name);

            if (fconfig.gender == "male")
            {
                entity.Attributes.Add("cott_gender", new OptionSetValue(455920000));
                
            }
            else if (fconfig.gender == "female")
            {
                entity.Attributes.Add("cott_gender", new OptionSetValue(455920001));
            }

            entity.Attributes.Add("cott_hometown", fconfig.hometown.name);
            entity.Attributes.Add("cott_hometownid", fconfig.hometown.id);
            entity.Attributes.Add("cott_locale", fconfig.locale);
            entity.Attributes.Add("cott_location", fconfig.location.name);
            entity.Attributes.Add("cott_locationid", fconfig.location.id);

            entity.Attributes.Add("cott_timezone", (int)((long)fconfig.timezone));

            entity.Attributes.Add("cott_profilepic", String.Format("https://graph.facebook.com/{0}/picture",fconfig.id ));

            service.Create(entity);
        }

        private static void UpdateFacebookConfig(dynamic fconfig, string token, DateTime expieres, EntityCollection response, IOrganizationService service)
        {
            var rentity = response.Entities[0];

            var entity = new Entity("cott_facebookconfig");
            entity.Attributes.Add("cott_accesstoken", token);
            entity.Attributes.Add("cott_name", fconfig.username);
            entity.Attributes.Add("cott_facebookuserid", fconfig.id);
            entity.Attributes.Add("cott_expieredate", expieres);
            entity.Attributes.Add("cott_email", fconfig.email);
            entity.Attributes.Add("cott_facebooklink", fconfig.link);

            entity.Attributes.Add("cott_firstname", fconfig.first_name);
            entity.Attributes.Add("cott_lastname", fconfig.last_name);

            if (fconfig.gender == "male")
            {
                entity.Attributes.Add("cott_gender", new OptionSetValue(455920000));

            }
            else if (fconfig.gender == "female")
            {
                entity.Attributes.Add("cott_gender", new OptionSetValue(455920001));
            }

            entity.Attributes.Add("cott_hometown", fconfig.hometown.name);
            entity.Attributes.Add("cott_hometownid", fconfig.hometown.id);
            entity.Attributes.Add("cott_locale", fconfig.locale);
            entity.Attributes.Add("cott_location", fconfig.location.name);
            entity.Attributes.Add("cott_locationid", fconfig.location.id);

            entity.Attributes.Add("cott_timezone", (int)((long)fconfig.timezone));
            
            entity.Attributes.Add("cott_profilepic", String.Format("https://graph.facebook.com/{0}/picture",fconfig.id ));

            entity.Id = rentity.Id;

            service.Update(entity);
        }

    }
}
