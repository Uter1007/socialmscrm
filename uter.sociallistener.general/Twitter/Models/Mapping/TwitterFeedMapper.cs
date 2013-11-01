using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.CRM.Models;
using Microsoft.Xrm.Sdk;
using Twitterizer;
using Twitterizer.Entities;
using LinqToTwitter;
using NLog;

namespace uter.sociallistener.general.Twitter.Models.Mapping
{
    public class TwitterFeedMapper
    {

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static TwitterFeed Map(Status status)
        {
            Log.Trace("Map Feed");
            if (!String.IsNullOrEmpty(status.StatusID))
            {

                AutoMapper.Mapper.CreateMap<Status, TwitterFeed>()
                   .ForMember(x => x.CRMID, opt => opt.Ignore())
                   .ForMember(x => x.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
                   .ForMember(x => x.Id, opt => opt.MapFrom(src => decimal.Parse(src.StatusID)))
                   .ForMember(x => x.InReplyToScreenName, opt => opt.MapFrom(src => src.InReplyToScreenName))
                   .ForMember(x => x.InReplyToStatusId, opt => opt.MapFrom(src => src.InReplyToStatusID != null ? decimal.Parse(src.InReplyToStatusID) : (decimal?)null))
                   .ForMember(x => x.InReplyToUserId, opt => opt.MapFrom(src => src.InReplyToUserID != null ? decimal.Parse(src.InReplyToUserID) : (decimal?)null))
                   .ForMember(x => x.IsFavorited, opt => opt.MapFrom(src => src.Favorited))
                   .ForMember(x => x.IsTruncated, opt => opt.MapFrom(src => src.Truncated))
                   .ForMember(x => x.RetweetCount, opt => opt.MapFrom(src => src.RetweetCount))
                   .ForMember(x => x.RetweetCountPlus, opt => opt.Ignore())
                   .ForMember(x => x.Retweeted, opt => opt.MapFrom(src => src.Retweeted))
                   .ForMember(x => x.Source, opt => opt.MapFrom(src => src.Source))
                   .ForMember(x => x.StringId, opt => opt.MapFrom(src => src.StatusID))
                   .ForMember(x => x.Text, opt => opt.MapFrom(src => src.Text))
                   .ForMember(x => x.User, opt => opt.MapFrom(src => TwitterUserMapper.Map(src.User)))
                   .ForMember(dest => dest.Entities, opt => opt.MapFrom(src => TwitterEntityCollectionMapper.Map(src.Entities)));

                AutoMapper.Mapper.AssertConfigurationIsValid();
                return AutoMapper.Mapper.Map<Status, TwitterFeed>(status);
            }
            else
            {
                Log.Trace("Status ID = NULL");
            }

            return null;
        }

        public static TwitterFeed Map(TwitterStatus status)
        {
            AutoMapper.Mapper.CreateMap<TwitterStatus, TwitterFeed>()
                .ForMember(x => x.CRMID, opt => opt.Ignore())
                .ForMember(dest => dest.Entities, opt => opt.MapFrom(src => TwitterEntityCollectionMapper.Map(src.Entities)));

            AutoMapper.Mapper.CreateMap<TwitterUser, TwitterUserProfile>().ForMember(x => x.CRMID, opt => opt.Ignore());

            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<TwitterStatus, TwitterFeed>(status);
        }

        public static Entity Map(TwitterFeed tweet, TwitterConfig config)
        {
            var crmtweet = new Entity("cott_twitterfeed");

            if (tweet.CRMID != null)
            {
                crmtweet.Id = tweet.CRMID;
            }

            crmtweet.Attributes.Add("subject", tweet.Text);
            crmtweet.Attributes.Add("cott_source", tweet.Source);
            crmtweet.Attributes.Add("cott_twitterdate", tweet.CreatedDate);
            crmtweet.Attributes.Add("cott_tweetid", tweet.Id.ToString());
            crmtweet.Attributes.Add("cott_twittername", tweet.User.ScreenName);
            crmtweet.Attributes.Add("cott_twitterid", tweet.User.Id.ToString());
            crmtweet.Attributes.Add("scheduledend", DateTime.Now);
            crmtweet.Attributes.Add("cott_direction", new OptionSetValue(455920000));
            crmtweet.Attributes.Add("cott_inreplytoscreenname", tweet.InReplyToScreenName != null ? tweet.InReplyToScreenName.ToString() : null);
            crmtweet.Attributes.Add("cott_inreplytostatusid", tweet.InReplyToStatusId != null ? tweet.InReplyToStatusId.ToString() : null);
            crmtweet.Attributes.Add("cott_inreplytouserid", tweet.InReplyToUserId != null ? tweet.InReplyToUserId.ToString() : null);
            crmtweet.Attributes.Add("cott_isfavorited", tweet.IsFavorited);
            crmtweet.Attributes.Add("cott_istruncated", tweet.IsTruncated);
            crmtweet.Attributes.Add("cott_retweetcount", tweet.RetweetCount);
            crmtweet.Attributes.Add("cott_retweetcountplus", tweet.RetweetCountPlus);
            crmtweet.Attributes.Add("cott_retweeted", tweet.Retweeted);

            crmtweet.Attributes.Add("cott_createdbyconfig", new EntityReference("cott_twitterconfig", config.CRMID));
            crmtweet.Attributes.Add("cott_twitteruser", new EntityReference("cott_twitteruser", tweet.User.CRMID));

            return crmtweet;

        }
    }
}
