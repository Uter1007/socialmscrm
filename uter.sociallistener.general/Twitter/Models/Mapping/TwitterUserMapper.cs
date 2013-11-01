using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Twitterizer;
using LinqToTwitter;

namespace uter.sociallistener.general.Twitter.Models.Mapping
{
    public class TwitterUserMapper
    {

        public static TwitterUserProfile Map(TwitterUser user)
        {
            AutoMapper.Mapper.CreateMap<TwitterUser, TwitterUserProfile>().ForMember(x => x.CRMID, opt => opt.Ignore());
            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<TwitterUser, TwitterUserProfile>(user);
        }

        public static TwitterUserProfile Map(User user)
        {
            AutoMapper.Mapper.CreateMap<User, TwitterUserProfile>()
                .ForMember(x => x.CRMID, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => decimal.Parse(src.Identifier.ID)))
                .ForMember(x => x.StringId, opt => opt.MapFrom(src => src.Identifier.ID))
                .ForMember(x => x.ScreenName, opt => opt.MapFrom(src => src.Identifier.ScreenName))
                .ForMember(x => x.DoesReceiveNotifications, opt => opt.MapFrom(src => src.Notifications))
                .ForMember(x => x.IsContributorsEnabled, opt => opt.MapFrom(src => src.ContributorsEnabled))
                .ForMember(x => x.IsFollowing, opt => opt.MapFrom(src => src.Following))
                .ForMember(x => x.IsFollowedBy, opt => opt.Ignore())
                .ForMember(x => x.IsGeoEnabled, opt => opt.MapFrom(src => src.GeoEnabled))
                .ForMember(x => x.IsProtected, opt => opt.MapFrom(src => src.Protected))
                .ForMember(x => x.Language, opt => opt.MapFrom(src => src.Lang))
                .ForMember(x => x.NumberOfFavorites, opt => opt.MapFrom(src => (long)src.FavoritesCount))
                .ForMember(x => x.NumberOfFollowers, opt => opt.MapFrom(src => (long)src.FollowersCount))
                .ForMember(x => x.NumberOfFriends, opt => opt.MapFrom(src => (long)src.FriendsCount))
                .ForMember(x => x.NumberOfStatuses, opt => opt.MapFrom(src => (long)src.StatusesCount))
                .ForMember(x => x.ProfileBackgroundImageLocation, opt => opt.MapFrom(src => src.ProfileBackgroundImageUrl))
                .ForMember(x => x.ProfileImageLocation, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(x => x.TimeZoneOffset, opt => opt.MapFrom(src => Convert.ToDouble(src.UtcOffset)))
                .ForMember(x => x.Website, opt => opt.MapFrom(src => src.Url));

            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<User, TwitterUserProfile>(user);
        }



        public static Entity Map(TwitterUserProfile user, Guid configId)
        {
            var tuser = new Entity("cott_twitteruser");
            if (user.CRMID != null)
            {
                tuser.Id = user.CRMID;

            }
            
            tuser.Attributes.Add("cott_userid", user.Id.ToString());
           
            tuser.Attributes.Add("cott_backgroundpic", user.ProfileBackgroundImageLocation);

            if (user.DoesReceiveNotifications != null)
            {
                tuser.Attributes.Add("cott_doesreceivenotifications", user.DoesReceiveNotifications);
            }

            tuser.Attributes.Add("cott_description", user.Description);

            tuser.Attributes.Add("cott_iscontributorsenabled", user.IsContributorsEnabled);

            if (user.IsFollowedBy != null)
            {
                tuser.Attributes.Add("cott_isfollowedby", user.IsFollowedBy);
            }

            if (user.IsFollowing != null)
            {
                tuser.Attributes.Add("cott_isfollowing", user.IsFollowing);
            }

            if (user.IsGeoEnabled != null)
            {
                tuser.Attributes.Add("cott_isgeoenabled", user.IsGeoEnabled);
            }


            tuser.Attributes.Add("cott_isprotected", user.IsProtected);
            tuser.Attributes.Add("cott_language", user.Language);

            tuser.Attributes.Add("cott_listedcount", user.ListedCount);

            tuser.Attributes.Add("cott_location", user.Location);

            tuser.Attributes.Add("cott_name", user.ScreenName);

            tuser.Attributes.Add("cott_numberoffavorites", (int)user.NumberOfFavorites);

            if (user.NumberOfFollowers != null)
            {
                tuser.Attributes.Add("cott_numberoffollowers", (int)user.NumberOfFollowers);
            }

           
            tuser.Attributes.Add("cott_numberoffriends", (int)user.NumberOfFriends);
            tuser.Attributes.Add("cott_numberofstatuses", (int)user.NumberOfStatuses);
            tuser.Attributes.Add("cott_personalname", user.Name);
            tuser.Attributes.Add("cott_timezone", user.TimeZone);

            tuser.Attributes.Add("cott_twitterpic", user.ProfileImageLocation);

            if (user.Verified != null)
            {
                tuser.Attributes.Add("cott_verified",user.Verified);
            }

            tuser.Attributes.Add("cott_website", user.Website);

            tuser.Attributes.Add("cott_createdbyconfig", new EntityReference("cott_twitterconfig", configId));

            return tuser;
        }
        
    }
}
