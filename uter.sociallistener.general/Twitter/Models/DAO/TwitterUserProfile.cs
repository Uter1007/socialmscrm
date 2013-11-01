using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Social;

namespace uter.sociallistener.general.Twitter.Models
{
    public class TwitterUserProfile : ISocialUser
    {

        public Guid CRMID { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }
        public string Description { get; set; }
        public decimal Id {get;set;}
        public string StringId { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }

        public Nullable<bool> DoesReceiveNotifications { get; set; }
        public bool IsContributorsEnabled { get; set; }
        public Nullable<bool> IsFollowing { get; set; }
        public Nullable<bool> IsFollowedBy { get; set; }
        public Nullable<bool> IsGeoEnabled { get; set; }
        public bool IsProtected { get; set; }
        public string Language { get; set; }

        public int ListedCount { get; set; }
        public string Location { get; set; }
        public long NumberOfFavorites { get; set; }
        public long NumberOfFriends { get; set; }
        public  Nullable<long> NumberOfFollowers { get; set; }
        public long NumberOfStatuses { get; set; }

        public Nullable<bool> Verified { get; set; } 
        public string ProfileBackgroundImageLocation { get; set; }
        public string ProfileImageLocation { get; set; }
        public string TimeZone { get; set; }
        public Nullable<double> TimeZoneOffset { get; set; }
        public string Website { get; set; }
    }
}
