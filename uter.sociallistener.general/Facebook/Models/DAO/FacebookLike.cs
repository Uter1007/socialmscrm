using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uter.sociallistener.general.Facebook.Models.DAO
{
    public class FacebookLike
    {
        public Guid CRMID { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }

        public Guid RelatedFeed { get; set; }
        public Guid FacebookUser { get; set; }
    }
}
