using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Twitterizer.Entities;
using LinqToTwitter;

namespace uter.sociallistener.general.Twitter.Models.Mapping
{
    public class TwitterUrlMapper
    {

        public static TwitterUrl Map(UrlEntity url)
        {
            AutoMapper.Mapper.CreateMap<UrlEntity, TwitterUrl>()
                .ForMember(x => x.CRMID, opt => opt.Ignore());
            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<UrlEntity, TwitterUrl>(url);
        }

        public static TwitterUrl Map(TwitterUrlEntity url)
        {
            AutoMapper.Mapper.CreateMap<TwitterUrlEntity, TwitterUrl>().ForMember(x => x.CRMID, opt => opt.Ignore());
            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<TwitterUrlEntity, TwitterUrl>(url);
        }

        public static Entity Map(TwitterUrl url, Guid configId)
        {
            var turl = new Entity("cott_twitterurl");
            if (url.CRMID != null)
            {
                turl.Id = url.CRMID;
            }
            turl.Attributes.Add("cott_name", url.Url);
            turl.Attributes.Add("cott_createdbyconfig", new EntityReference("cott_twitterconfig", configId));

            return turl;
        }
    }
}
