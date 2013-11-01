using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uter.sociallistener.general.Facebook.Models.DAO
{
    public class FacebookComment
    {
        public Guid CRMID { get; set; }
        public string ID { get; set; }
        public string FromID { get; set; }
        public string FromName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<FacebookLike> Likes { get; set; }

        public Guid RelatedFeed { get; set; }
        public Guid FacebookUser { get; set; }

    }
}
