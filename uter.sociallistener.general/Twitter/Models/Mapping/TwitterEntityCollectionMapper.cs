using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitterizer.Entities;
using LinqToTwitter;

namespace uter.sociallistener.general.Twitter.Models.Mapping
{
    public class TwitterEntityCollectionMapper
    {
        public static TwitterSocialEntity Map(TwitterEntity entity)
        {
            if (entity.GetType() == typeof(TwitterHashTagEntity))
            {
                return TwitterHashTagMapper.Map((TwitterHashTagEntity)entity);
            }

            else if (entity.GetType() == typeof(TwitterUrlEntity))
            {
                return TwitterUrlMapper.Map((TwitterUrlEntity)entity);
            }

            else if (entity.GetType() == typeof(TwitterMentionEntity))
            {
                return TwitterMentionMapper.Map((TwitterMentionEntity)entity);
            }
            else return null;

        }

        public static IList<TwitterSocialEntity> Map(TwitterEntityCollection entitycollection)
        {
            var list = new List<TwitterSocialEntity>();

            foreach(var entity in entitycollection){
                var e = Map(entity);
                if (e != null)
                    list.Add(e);
            }

            return list;
        }

        public static IList<TwitterSocialEntity> Map(Entities entitycollection)
        {
            var list = new List<TwitterSocialEntity>();

            foreach (var entity in entitycollection.HashTagEntities)
            {
                var e = TwitterHashTagMapper.Map(entity);
                if (e != null)
                    list.Add(e);
            }

            foreach (var entity in entitycollection.UrlEntities)
            {
                var e = TwitterUrlMapper.Map(entity);
                if (e != null)
                    list.Add(e);
            }

            foreach (var entity in entitycollection.UserMentionEntities)
            {
                var e = TwitterMentionMapper.Map(entity);
                if (e != null)
                    list.Add(e);
            }

            return list;
        }
    }
}
