using Facebook;
using Microsoft.Xrm.Sdk;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Facebook.Models.DAO;

namespace uter.sociallistener.general.Facebook.Models.Mapping
{
    public class FacebookFeedMapper
    {

        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public static FacebookFeed Map(JsonObject fbfeed)
        {

            Log.Debug("Map Facebook Feed from JsonObject");

            var castobj = (IDictionary<String, object>)fbfeed;
            var feed = new FacebookFeed();

            if (castobj.ContainsKey("id"))
            {
                feed.ID = (string)castobj["id"];
            }

            if (castobj.ContainsKey("from"))
            {
                var castfrom = (IDictionary<String, object>)castobj["from"];

                if (castfrom.ContainsKey("name"))
                {
                    feed.FromName = (string)castfrom["name"];

                }

                if (castfrom.ContainsKey("id"))
                {
                    feed.FromID = (string)castfrom["id"];

                }
            }

            if (castobj.ContainsKey("story"))
            {
                feed.Story = (string)castobj["story"];
            }

            if (castobj.ContainsKey("caption"))
            {
                feed.Caption = (string)castobj["caption"];
            }

            if (castobj.ContainsKey("message"))
            {
                feed.Message = (string)castobj["message"];
            }

            if (castobj.ContainsKey("picture"))
            {
                feed.Picture = (string)castobj["picture"];
            }

            if (castobj.ContainsKey("link"))
            {
                feed.Link = (string)castobj["link"];
            }

            if (castobj.ContainsKey("source"))
            {
                feed.Source = (string)castobj["source"];
            }

            if (castobj.ContainsKey("name"))
            {
                feed.Name = (string)castobj["name"];
            }

            if (castobj.ContainsKey("icon"))
            {
                feed.Icon = (string)castobj["icon"];
            }

            if (castobj.ContainsKey("privacy"))
            {
                var castpriv = (IDictionary<String, object>)castobj["privacy"];
                if (castpriv.ContainsKey("description"))
                {
                    feed.PrivacyDescription = (string)castpriv["description"];
                }

                if (castpriv.ContainsKey("value"))
                {
                    feed.PrivacyValue = (string)castpriv["value"];
                }

            }

            if (castobj.ContainsKey("type"))
            {
                feed.Type = (string)castobj["type"];
            }

            if (castobj.ContainsKey("status_type"))
            {
                feed.StatusType = (string)castobj["status_type"];
            }

            if (castobj.ContainsKey("created_time"))
            {
                var createtime = DateTime.Parse((string)castobj["created_time"]);
                if (createtime > new DateTime(1901, 1, 1) && createtime < new DateTime(2030, 1, 1))
                {
                    feed.CreatedTime = createtime.ToUniversalTime();
                }
            }

            if (castobj.ContainsKey("updated_time"))
            {
                var updatedtime = DateTime.Parse((string)castobj["updated_time"]);
                if (updatedtime > new DateTime(1901, 1, 1) && updatedtime < new DateTime(2030, 1, 1))
                {
                    feed.UpdatedTime = updatedtime.ToUniversalTime();
                }
            }

            if (castobj.ContainsKey("application"))
            {
                var castapplication = (IDictionary<String, object>)castobj["application"];
                if (castapplication.ContainsKey("name"))
                {
                    feed.ApplicationName = (string)castapplication["name"];
                }

                if (castapplication.ContainsKey("id"))
                {
                    feed.ApplicationID = (string)castapplication["id"];
                }

            }

            if (castobj.ContainsKey("comments"))
            {
                var castcomments = (IDictionary<String, object>)castobj["comments"];

                if (castcomments.ContainsKey("data"))
                {
                    var data = (JsonArray)castcomments["data"];

                    var comments = new List<FacebookComment>();

                    foreach (var dobj in data)
                    {
                        var comment = MapToComment((JsonObject)dobj);
                        if (comment != null)
                        {
                            comments.Add(comment);
                        }
                    }

                    feed.Comments = comments;
                }
            }

            if (castobj.ContainsKey("likes"))
            {
                var castlikes = (IDictionary<String, object>)castobj["likes"];

                if (castlikes.ContainsKey("data"))
                {
                    var data = (JsonArray)castlikes["data"];

                    var likes = new List<FacebookLike>();

                    foreach (var dobj in data)
                    {
                        var like = MapToLike((JsonObject)dobj);
                        if (like != null)
                        {
                            likes.Add(like);
                        }
                    }

                    feed.Likes = likes;
                }
            }

            return feed;

        }

        public static FacebookLike MapToLike(JsonObject obj)
        {
            var castlike = (IDictionary<String, object>)obj;

            var like = new FacebookLike();

            if (castlike.ContainsKey("id"))
            {
                like.ID = (string)castlike["id"];
            }

            if (castlike.ContainsKey("name"))
            {
                like.Name = (string)castlike["name"];
            }

            return like;
        }

        public static FacebookComment MapToComment(JsonObject obj)
        {
            var castcomment = (IDictionary<String, object>)obj;

            var comment = new FacebookComment();

            if (castcomment.ContainsKey("id"))
            {
                comment.ID = (string)castcomment["id"];
            }

            if (castcomment.ContainsKey("message"))
            {
                comment.Message = (string)castcomment["message"];
            }

            if (castcomment.ContainsKey("created_time"))
            {
                comment.CreatedTime = DateTime.Parse((string)castcomment["created_time"]);
            }

            if (castcomment.ContainsKey("from"))
            {
                var castfrom = (IDictionary<String, object>)castcomment["from"];

                if (castfrom.ContainsKey("name"))
                {
                    comment.FromName = (string)castfrom["name"];

                }

                if (castfrom.ContainsKey("id"))
                {
                    comment.FromID = (string)castfrom["id"];

                }
            }

            //if (castcomment.ContainsKey("likes"))
            //{
            //    var castlikes = (IDictionary<String, object>)castcomment["likes"];

            //    if (castlikes.ContainsKey("data"))
            //    {
            //        var data = (JsonArray)castlikes["data"];

            //        var likes = new List<Like>();

            //        foreach (var dobj in data)
            //        {
            //            var like = MapToLike((JsonObject)dobj);
            //            if (like != null)
            //            {
            //                likes.Add(like);
            //            }
            //        }

            //        comment.Likes = likes;
            //    }
            //}

            return comment;
        }

        public static Entity Map(FacebookFeed feed, FacebookConfig config)
        {
            var entity = new Entity("cott_facebookfeed");

            if (feed.RelatedFeed != Guid.Empty)
            {
                entity.Attributes.Add("cott_relatedfbfeed", new EntityReference("cott_facebookfeed", (Guid)feed.RelatedFeed));
            }

            entity.Attributes.Add("cott_appid", feed.ApplicationID);
            entity.Attributes.Add("cott_appname", feed.ApplicationName);
            entity.Attributes.Add("cott_caption", feed.Caption);

            if (feed.Comments != null)
            {
                entity.Attributes.Add("cott_comment_count", feed.Comments.Count());

            }
            else
            {
                entity.Attributes.Add("cott_comment_count", 0);
            }

            entity.Attributes.Add("cott_config", new EntityReference("cott_facebookconfig", config.CRMID));


            if (feed.CreatedTime > new DateTime(1901, 1, 1) && feed.CreatedTime < new DateTime(2030, 1, 1))
            {
                entity.Attributes.Add("cott_createdtime", feed.CreatedTime.ToUniversalTime());
            }

            if (feed.Story != null)
            {
                if (feed.Story.Length > 140)
                {
                    entity.Attributes.Add("subject", feed.Story.Substring(0, 140));
                    entity.Attributes.Add("description", feed.Story);
                }
                else
                {
                    entity.Attributes.Add("subject", feed.Story);
                    entity.Attributes.Add("description", feed.Story);
                }
            }
            else if (feed.Message != null)
            {
                if (feed.Message.Length > 140)
                {
                    entity.Attributes.Add("subject", feed.Message.Substring(0, 140));
                    entity.Attributes.Add("description", feed.Message);
                }
                else
                {
                    entity.Attributes.Add("subject", feed.Message);
                    entity.Attributes.Add("description", feed.Message);
                }
            }
            else if (feed.Name != null)
            {
                if (feed.Name.Length > 140)
                {
                    entity.Attributes.Add("subject", feed.Name.Substring(0, 140));
                    entity.Attributes.Add("description", feed.Name);
                }
                else
                {
                    entity.Attributes.Add("subject", feed.Name);
                    entity.Attributes.Add("description", feed.Name);
                }
            }
            else if (feed.Source != null)
            {
                if (feed.Source.Length > 140)
                {
                    entity.Attributes.Add("subject", feed.Source.Substring(0, 140));
                    entity.Attributes.Add("description", feed.Source);
                }
                else
                {
                    entity.Attributes.Add("subject", feed.Source);
                    entity.Attributes.Add("description", feed.Source);
                }
            }
            entity.Attributes.Add("cott_facebook_statusid", feed.ID);
            entity.Attributes.Add("cott_facebook_userid", feed.FromID);
            entity.Attributes.Add("cott_facebook_username", feed.FromName);

            entity.Attributes.Add("cott_facebookuser", new EntityReference("cott_facebookuser", (Guid)feed.FacebookUser));

            entity.Attributes.Add("cott_link", feed.Link);
            if (feed.Likes != null)
            {
                entity.Attributes.Add("cott_like_count", feed.Likes.Count());
            }
            else
            {
                entity.Attributes.Add("cott_like_count", 0);
            }

            entity.Attributes.Add("cott_picture", feed.Picture);
            entity.Attributes.Add("cott_privacy_description", feed.PrivacyDescription);
            entity.Attributes.Add("cott_privacy_value", feed.PrivacyValue);
            entity.Attributes.Add("cott_source", feed.Source);
            entity.Attributes.Add("cott_story", feed.Story);
            entity.Attributes.Add("cott_type", feed.Type);

            if (feed.UpdatedTime > new DateTime(1901, 1, 1) && feed.UpdatedTime < new DateTime(2030, 1, 1))
            {
                entity.Attributes.Add("cott_updatedtime", feed.UpdatedTime.ToUniversalTime());
            }

            if (feed.CRMID != Guid.Empty)
            {
                entity.Id = feed.CRMID;
            }

            return entity;
        }

        public static Entity MapToComment(FacebookComment comment, FacebookConfig config)
        {
            var commententity = new Entity("cott_facebookfeed");

            if (comment.ID != null)
            {
                commententity.Attributes.Add("cott_facebook_statusid", comment.ID);
            }

            if (comment.FromID != null)
            {
                commententity.Attributes.Add("cott_facebook_userid", comment.FromID);
            }

            if (comment.FromName != null)
            {
                commententity.Attributes.Add("cott_facebook_username", comment.FromName);
            }

            
            commententity.Attributes.Add("cott_iscomment", true);

            if (comment.CreatedTime > new DateTime(1990, 1, 1))
            {
                commententity.Attributes.Add("cott_createdtime", comment.CreatedTime);
            }

            if (comment.RelatedFeed != Guid.Empty)
            {
                commententity.Attributes.Add("cott_relatedfbfeed", new EntityReference("cott_facebookfeed", (Guid)comment.RelatedFeed));
            }

            if (config.CRMID != Guid.Empty)
            {
                commententity.Attributes.Add("cott_config", new EntityReference("cott_facebookconfig", config.CRMID));
            }


            if (comment.Message != null && comment.Message.Length > 140)
            {
                commententity.Attributes.Add("subject", comment.Message.Substring(0, 140));
                commententity.Attributes.Add("description", comment.Message);
            }
            else
            {
                commententity.Attributes.Add("subject", comment.Message);
                commententity.Attributes.Add("description", comment.Message);
            }

            if (comment.CRMID != Guid.Empty)
            {
                commententity.Id = comment.CRMID;
            }

            return commententity;
        }

        public static Entity MapToLike(FacebookLike like, FacebookConfig config)
        {
            var likeentity = new Entity("cott_fblike");

            if (like.ID != null)
            {
                likeentity.Attributes.Add("cott_facebook_userid", like.ID);
            }

            if (like.FacebookUser != Guid.Empty)
            {
                likeentity.Attributes.Add("cott_fbuser", new EntityReference("cott_facebookuser", (Guid)like.FacebookUser));
            }

            if (like.Name != null)
            {
                likeentity.Attributes.Add("cott_facebook_username", like.Name);
            }

            if (config.CRMID != Guid.Empty)
            {
                likeentity.Attributes.Add("cott_facebook_config", new EntityReference("cott_facebookconfig", config.CRMID));
            }

            if (like.RelatedFeed != Guid.Empty)
            {
                likeentity.Attributes.Add("cott_relatedfeed", new EntityReference("cott_facebookfeed", (Guid)like.RelatedFeed));
            }

            if (like.CRMID != Guid.Empty)
            {
                likeentity.Id = like.CRMID;
            }

            return likeentity;

        }
    }
}
