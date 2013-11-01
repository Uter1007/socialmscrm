using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Social;
using Microsoft.Xrm.Sdk;

namespace uter.sociallistener.general.CRM.Models
{
    public class TwitterConfig : ICRMModel
    {
        public string Location { get; set; }
        public string Accesstoken { get; set; }
        public string Accesstokensecret { get; set; }
        public string Website { get; set; }
        public string PersonalName { get; set; }
        public string TwitterPic { get; set; }
        public Guid CRMID { get; set; }
        public string LastStatusId { get; set; }
        public int MaxRecords { get; set; }

    }
}
