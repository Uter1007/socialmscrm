﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Twitter.Models.Mapping;

namespace uter.sociallistener.general.Twitter.Models
{
    public class TwitterUrl : TwitterSocialEntity, ITwitterEntity
    {
        public Guid CRMID { get; set; }
        public string Url { get; set; }
    }

     
}
