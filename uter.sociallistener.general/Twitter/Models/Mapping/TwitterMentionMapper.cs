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
    public class TwitterMentionMapper
    {

        public static TwitterMention Map(UserMentionEntity mention)
        {
            AutoMapper.Mapper.CreateMap<UserMentionEntity, TwitterMention>()
                .ForMember(x => x.CRMID, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.MapFrom(src => src.Id));
            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<UserMentionEntity, TwitterMention>(mention);
        }

        public static TwitterMention Map(TwitterMentionEntity mention)
        {
            AutoMapper.Mapper.CreateMap<TwitterMentionEntity, TwitterMention>().ForMember(x => x.CRMID, opt => opt.Ignore());
            AutoMapper.Mapper.AssertConfigurationIsValid();
            return AutoMapper.Mapper.Map<TwitterMentionEntity, TwitterMention>(mention);
        }

        public static Entity Map(TwitterMention mention, Guid configId)
        {

            var tuser = new Entity("cott_twitteruser");
            if (mention.CRMID != null)
            {
                tuser.Id = mention.CRMID;
            }
            tuser.Attributes.Add("cott_name", mention.ScreenName);
            tuser.Attributes.Add("cott_userid", mention.UserId.ToString());
            tuser.Attributes.Add("cott_createdbyconfig", new EntityReference("cott_twitterconfig", configId));

            return tuser;
        }
       
    }
}
