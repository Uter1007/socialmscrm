using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uter.sociallistener.general.Facebook.Models.DAO
{
    public class FacebookUser
    {
        public Guid CRMID { get; set; }
        public string ID { get; set; }
        public string Pic { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Link { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string RelationshipStatus { get; set; }
        public string WebSite { get; set; }
        public string Locale { get; set; }
        public DateTime UpdatedTime { get; set; }

        
    }
}
