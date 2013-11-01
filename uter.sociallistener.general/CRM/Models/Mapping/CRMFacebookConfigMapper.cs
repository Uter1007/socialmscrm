using Microsoft.Xrm.Sdk;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Facebook.Models.DAO;

namespace uter.sociallistener.general.CRM.Models.Mapping
{
    public class CRMFacebookConfigMapper
    {

        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public static Entity Map(FacebookConfig fconfig)
        {

            Log.Debug("Map CRM Config");

            var entity = new Entity("cott_facebookconfig");

            if (fconfig.Accesstoken != null)
            {
                entity.Attributes.Add("cott_accesstoken", fconfig.Accesstoken);
            }


            if (fconfig.UserName != null)
            {
                entity.Attributes.Add("cott_name", fconfig.UserName);
            }

            if (fconfig.UserId != null){
                entity.Attributes.Add("cott_facebookuserid", fconfig.UserId);
            }

            if (fconfig.ExpireDate != null)
            {
                entity.Attributes.Add("cott_expieredate", fconfig.ExpireDate);
            }

            if (fconfig.Email != null)
            {
                entity.Attributes.Add("cott_email", fconfig.Email);
            }

            if (fconfig.Link != null)
            {
                entity.Attributes.Add("cott_facebooklink", fconfig.Link);
            }

            if (fconfig.FirstName != null)
            {
                entity.Attributes.Add("cott_firstname", fconfig.FirstName);
            }

            if (fconfig.LastName != null)
            {
                entity.Attributes.Add("cott_lastname", fconfig.LastName);
            }

            //Male = new OptionSetValue(455920000) | Female = new OptionSetValue(455920001)
            if (fconfig.Gender == 455920000 || fconfig.Gender == 455920001)
            {
                entity.Attributes.Add("cott_gender", new OptionSetValue(fconfig.Gender));
            }

            if (fconfig.HomeTownName != null)
            {
                entity.Attributes.Add("cott_hometown", fconfig.HomeTownName);
            }

            if (fconfig.HomeTownId != null)
            {
                entity.Attributes.Add("cott_hometownid", fconfig.HomeTownId);
            }

            if (fconfig.Locale != null)
            {
                entity.Attributes.Add("cott_locale", fconfig.Locale);
            }

            if (fconfig.LocationName != null)
            {
                entity.Attributes.Add("cott_location", fconfig.LocationName);
            }

            if (fconfig.LocationId != null)
            {
                entity.Attributes.Add("cott_locationid", fconfig.LocationId);
            }

            if (fconfig.Timezone != 0)
            {
                entity.Attributes.Add("cott_timezone", fconfig.Timezone);
            }

            if (fconfig.UserId != null){
                entity.Attributes.Add("cott_profilepic", String.Format("https://graph.facebook.com/{0}/picture", fconfig.UserId));
            }

            if (fconfig.LastStatusDateTime > new DateTime(1990, 1, 1))
            {
                entity.Attributes.Add("cott_laststatusdate",fconfig.LastStatusDateTime);
            }

            if (fconfig.CRMID != Guid.Empty)
            {
                Log.Debug("Set ID");
                entity.Id = fconfig.CRMID;

            }
            else
            {
                Log.Warn("Warning no CRM GUID was given - will throw error");
            }

            Log.Debug("Return mapped Config");
            return entity;

        }

        public static FacebookConfig Map(Entity config)
        {

            var fbconfig = new FacebookConfig();

            if (config.Contains("cott_accesstoken"))
            {
                fbconfig.Accesstoken = (string)config["cott_accesstoken"];
            }

            if (config.Contains("cott_laststatusdate"))
            {
                fbconfig.LastStatusDateTime = (DateTime)config["cott_laststatusdate"];
            }

            if (config.Contains("cott_name"))
            {
                fbconfig.UserName = (string)config["cott_name"];
            }

            if (config.Contains("cott_facebookuserid"))
            {
                fbconfig.UserId = (string)config["cott_facebookuserid"];
            }

            if (config.Contains("cott_expieredate"))
            {
                fbconfig.ExpireDate = (DateTime)config["cott_expieredate"];
            }

            if (config.Contains("cott_email"))
            {
                fbconfig.Email = (string)config["cott_email"];
            }
            
            if (config.Contains("cott_facebooklink"))
            {
                fbconfig.Link = (string)config["cott_facebooklink"];
            }

            if (config.Contains("cott_firstname"))
            {
                fbconfig.FirstName = (string)config["cott_firstname"];
            }

            if (config.Contains("cott_lastname"))
            {
                fbconfig.LastName = (string)config["cott_lastname"];
            }

            if (config.Contains("cott_needrefresh"))
            {
                fbconfig.NeedRefresh = (bool)config["cott_needrefresh"];
            }

            if (config.Contains("cott_profilepic"))
            {
                fbconfig.FacebookPic = (string)config["cott_profilepic"];
            }

            fbconfig.CRMID = config.Id;

            return fbconfig;

        }
    }
}
