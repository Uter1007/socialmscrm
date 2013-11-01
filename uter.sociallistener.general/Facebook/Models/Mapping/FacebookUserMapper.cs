using Facebook;
using Microsoft.Xrm.Sdk;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Facebook.Models.DAO;

namespace uter.sociallistener.general.Facebook.Models.Mapping
{
    public class FacebookUserMapper
    {

        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public static FacebookUser Map(dynamic fbuser, dynamic fbuserpic){

            Log.Debug("Map Facebook User");
            var fuser = Map(fbuser);
            
            if (fbuserpic.picture != null && fbuserpic.picture.data != null && fbuserpic.picture.data.url != null)
            {
                fuser.Pic = fbuserpic.picture.data.url;
            }

            return fuser;
        }

        public static Entity Map(FacebookUser user, FacebookConfig config)
        {
            var tuser = new Entity("cott_facebookuser");

            if (user.UserName != null)
            {
                tuser.Attributes.Add("cott_name", user.UserName);
            }

            if (user.FirstName != null)
            {
                tuser.Attributes.Add("cott_firstname", user.FirstName);
            }

            if (user.LastName != null)
            {
                tuser.Attributes.Add("cott_lastname", user.LastName);
            }

            if (user.Birthday > new DateTime(1990, 1, 1))
            {
                tuser.Attributes.Add("cott_birthday", user.Birthday);
            }

            if (user.ID != null)
            {
                tuser.Attributes.Add("cott_userid", user.ID.ToString());
            }

            if (user.Pic != null)
            {
                tuser.Attributes.Add("cott_facebookpic", user.Pic);
            }

            if (config.CRMID != null)
            {
                tuser.Attributes.Add("cott_config", new EntityReference("cott_facebookconfig", config.CRMID));
            }

            if (user.Link != null)
            {
                tuser.Attributes.Add("cott_facebookurl", user.Link);
            }

            if (user.WebSite != null)
            {
                tuser.Attributes.Add("cott_website", user.WebSite);
            }

            if (user.UpdatedTime > new DateTime(1990, 1, 1))
            {
                tuser.Attributes.Add("cott_updatedtime", user.UpdatedTime);
            }

            if (user.Locale != null)
            {
                tuser.Attributes.Add("cott_locale", user.Locale);
            }
            
            if (user.CRMID != null)
            {
                tuser.Id = user.CRMID;

            }

            return tuser;
        }

        public static FacebookUser Map(dynamic fbuser)
        {

            Log.Debug("Map Facebook User Detail");
            var muser = new FacebookUser();
            
            if (fbuser.id != null)
            {
                muser.ID = fbuser.id;
            }

            if (fbuser.name != null)
            {
                muser.Name = fbuser.name;
            }

            if (fbuser.first_name != null)
            {
                muser.FirstName = fbuser.first_name;
            }
            if (fbuser.last_name != null)
            {
                muser.LastName = fbuser.last_name;
            }

            if (fbuser.link != null)
            {
                muser.Link = fbuser.link;
            }

            if (fbuser.username != null)
            {
                muser.UserName = fbuser.username;
            }

            if (fbuser.birthday != null)
            {
                muser.Birthday = DateTime.Parse(fbuser.birthday, new CultureInfo("de-DE", false));
            }

            if (fbuser.website != null)
            {
                muser.WebSite = fbuser.website;
            }

            if (fbuser.locale != null)
            {
                muser.Locale = fbuser.locale;
            }

            if (fbuser.updated_time != null)
            {
                muser.UpdatedTime = DateTime.Parse(fbuser.updated_time, new CultureInfo("de-DE", false));
            }

            Log.Debug("Return Facebook User");

            return muser;
        }
    }
}
