using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Social;
using uter.sociallistener.general.Twitter.Models.Mapping;

namespace uter.sociallistener.general.Twitter.Models
{
    public class TwitterFeed : ISocialFeed
    {
        public Guid CRMID { get; set; }
        public DateTime CreatedDate { get; set; }
        //public TwitterEntityCollection Entities { get; set; }
        public IList<TwitterSocialEntity> Entities { get; set; }
        public decimal Id { get; set; }
        public string InReplyToScreenName { get; set; }
        public Nullable<decimal> InReplyToStatusId { get; set; }
        public Nullable<decimal> InReplyToUserId { get; set; }
        public Nullable<bool> IsFavorited { get; set; }
        public Nullable<bool> IsTruncated { get; set; }
        public Nullable<int> RetweetCount { get; set; }
        public Nullable<bool> RetweetCountPlus { get; set; }
        public bool Retweeted { get; set; }
        public string Source { get; set; }
        public string StringId { get; set; }
        public string Text { get; set; }
        public TwitterUserProfile User { get; set; }
    }
}
