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
    public class TwitterHashTagMapper
    {

        public static TwitterHashTag Map(HashTagEntity hashtag)
        {
            AutoMapper.Mapper.CreateMap<HashTagEntity, TwitterHashTag>()
                .ForMember(x => x.CRMID, opt => opt.Ignore())
                .ForMember(x => x.Text, opt => opt.MapFrom(src => src.Tag));
            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<HashTagEntity, TwitterHashTag>(hashtag);
        }

        public static TwitterHashTag Map(TwitterHashTagEntity hashtag)
        {
            AutoMapper.Mapper.CreateMap<TwitterHashTagEntity, TwitterHashTag>().ForMember(x => x.CRMID, opt => opt.Ignore());
            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<TwitterHashTagEntity, TwitterHashTag>(hashtag);
        }

        public static Entity Map(TwitterHashTag hashtag, Guid configId)
        {
            var hash = new Entity("cott_twitterhashtag");
            if (hashtag.CRMID != null)
            {
                hash.Id = hashtag.CRMID;
            }
            hash.Attributes.Add("cott_name", hashtag.Text);
            hash.Attributes.Add("cott_createdbyconfig", new EntityReference("cott_twitterconfig", configId));

            return hash;
        }

    }
}
