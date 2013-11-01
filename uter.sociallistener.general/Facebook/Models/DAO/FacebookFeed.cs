using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uter.sociallistener.general.Facebook.Models.DAO
{
    public class FacebookFeed
    {

        public Guid CRMID { get; set; }
        public string ID { get; set; }
        public string Story { get; set; }
        public string FromName { get; set; }
        public string FromID { get; set; }
        public string Type { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationID { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string PrivacyDescription { get; set; }
        public string PrivacyValue { get; set; }
        public string StatusType { get; set; }

        public string Caption { get; set; }
        public string Message { get; set; }
        public string Picture { get; set; }
        public string Link { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }

        public List<FacebookLike> Likes { get; set; }
        public List<FacebookComment> Comments { get; set; }

        public Guid RelatedFeed { get; set; }
        public Guid FacebookUser { get; set; }
    }
}
