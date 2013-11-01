using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace uter.sociallistener.general.CRM.Models.Mapping
{
    public class CRMTwitterConfigMapper
    {
        public static Entity Map(TwitterConfig config)
        {
            var entity = new Entity("cott_twitterconfig");

            if (config.CRMID != null)
            {
                entity.Id = config.CRMID;
            }
            entity.Attributes.Add("cott_location", config.Location);
            entity.Attributes.Add("cott_website", config.Website);
            entity.Attributes.Add("cott_personalname", config.PersonalName);
            entity.Attributes.Add("cott_twitterpic", config.TwitterPic);
            entity.Attributes.Add("cott_accesstoken", config.Accesstoken);
            entity.Attributes.Add("cott_accesstokensecret", config.Accesstokensecret);
            entity.Attributes.Add("cott_lastid", config.LastStatusId);
            entity.Attributes.Add("cott_count", config.MaxRecords);

            return entity;
        }

        public static TwitterConfig Map(Entity entity)
        {

            var config = new TwitterConfig();

            config.CRMID = entity.Id;

            if (entity.Contains("cott_accesstoken"))
            {
                config.Accesstoken = (string)entity["cott_accesstoken"];
            }
            if (entity.Contains("cott_accesstokensecret"))
            {
                config.Accesstokensecret = (string)entity["cott_accesstokensecret"];
            }
            if (entity.Contains("cott_location"))
            {
                config.Location = (string)entity["cott_location"];
            }
            if (entity.Contains("cott_website"))
            {
                config.Website = (string)entity["cott_website"];
            }
            if (entity.Contains("cott_personalname"))
            {
                config.PersonalName = (string)entity["cott_personalname"];
            }
            if (entity.Contains("cott_twitterpic"))
            {
                config.TwitterPic = (string)entity["cott_twitterpic"];
            }
            if (entity.Contains("cott_lastid"))
            {

                config.LastStatusId = (string)entity["cott_lastid"];
            }
            if (entity.Contains("cott_count"))
            {
                config.MaxRecords = (int)entity["cott_count"];
            }

           
            return config;
        }
    }
}
