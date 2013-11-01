using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uter.sociallistener.general.Facebook.Models.DAO
{
    public class FacebookConfig
    {
        public string Accesstoken { get; set; }

        public string FacebookPic { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Link { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }

        public string Email { get; set; }

        public DateTime ExpireDate { get; set; }

        public Guid CRMID { get; set; }
        
        public DateTime LastStatusDateTime { get; set; }

        public bool NeedRefresh { get; set; }

        public int Gender { get; set; }

        public string HomeTownName { get; set; }
        public string HomeTownId { get; set; }

        public string LocationName { get; set; }
        public string LocationId { get; set; }

        public string Locale { get; set; }

        public int Timezone { get; set; }
    }
}
