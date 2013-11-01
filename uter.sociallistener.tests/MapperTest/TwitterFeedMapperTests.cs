using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitterizer;
using Twitterizer.Entities;

namespace uter.sociallistener.tests.MapperTest
{
    [TestClass]
    public class TwitterFeedMapperTests
    {
        [TestMethod]
        public void TwitterFeedMapTest()
        {

            var twitteruser = new TwitterUser()
            {
                Id = (decimal)123,
                CreatedDate = new DateTime(2012, 1, 1),
                Description = "Test",
                DoesReceiveNotifications = false,
                FollowRequestSent = false,
                IsContributorsEnabled = true,
                IsFollowedBy = false,
                IsFollowing = true,
                IsGeoEnabled = false,
                IsProtected = false,
                Language = "German",
                ListedCount = 0,
                NumberOfFavorites = 0,
                NumberOfFollowers = 2,
                NumberOfFriends = 3,
                NumberOfStatuses = 10,
                ScreenName = "Testuser",
                StringId = "123",
                TimeZone = "GMT+1",
                Website = "www.orf.at"
            };

            var twitterfeed = new TwitterStatus()
            {
                Id = (decimal)123,
                CreatedDate = new DateTime(2012, 1, 1),
                Entities = new TwitterEntityCollection() { new TwitterHashTagEntity() { Text = "HashTag1" } },
                InReplyToScreenName = "testuser1",
                InReplyToStatusId = (decimal)5555,
                InReplyToUserId = (decimal)456,
                IsFavorited = true,
                IsTruncated = false,
                Retweeted = false,
                Source = "testsource",
                StringId = "123",
                Text = "that's a test",
                User = twitteruser
            };

            var feed = TwitterFeedMapper.Map(twitterfeed);

            Assert.AreEqual(twitterfeed.Id, feed.Id);
            Assert.AreEqual(twitterfeed.CreatedDate, feed.CreatedDate);
            Assert.AreEqual(twitterfeed.InReplyToScreenName, feed.InReplyToScreenName);
            Assert.AreEqual(twitterfeed.InReplyToStatusId, feed.InReplyToStatusId);
            Assert.AreEqual(twitterfeed.InReplyToUserId, feed.InReplyToUserId);
            Assert.AreEqual(twitterfeed.IsFavorited, feed.IsFavorited);
            Assert.AreEqual(twitterfeed.IsTruncated, feed.IsTruncated);
            Assert.AreEqual(twitterfeed.RetweetCount, feed.RetweetCount);
            Assert.AreEqual(twitterfeed.RetweetCountPlus, feed.RetweetCountPlus);
            Assert.AreEqual(twitterfeed.IsTruncated, feed.IsTruncated);
            Assert.AreEqual(twitterfeed.Retweeted, feed.Retweeted);
            Assert.AreEqual(twitterfeed.Source, feed.Source);
            Assert.AreEqual(twitterfeed.StringId, feed.StringId);
            Assert.AreEqual(twitterfeed.Text, feed.Text);

            Assert.AreEqual(twitterfeed.Entities.Count, 1);
            Assert.AreEqual(feed.Entities.Count, 1);
            Assert.AreEqual(feed.Entities.GetType(), typeof(List<TwitterSocialEntity>));

            Assert.AreEqual(twitteruser.Id, feed.User.Id);
            Assert.AreEqual(twitteruser.CreatedDate, feed.User.CreatedDate);
            Assert.AreEqual(twitteruser.Description, feed.User.Description);
            Assert.AreEqual(twitteruser.DoesReceiveNotifications, feed.User.DoesReceiveNotifications);
            //Assert.AreEqual(twitteruser.FollowRequestSent, tweetprofile.FollowRequestSent);
            Assert.AreEqual(twitteruser.IsContributorsEnabled, feed.User.IsContributorsEnabled);
            Assert.AreEqual(twitteruser.IsFollowedBy, feed.User.IsFollowedBy);
            Assert.AreEqual(twitteruser.IsFollowing, feed.User.IsFollowing);

            Assert.AreEqual(twitteruser.IsGeoEnabled, feed.User.IsGeoEnabled);
            Assert.AreEqual(twitteruser.IsProtected, feed.User.IsProtected);
            Assert.AreEqual(twitteruser.Language, feed.User.Language);
            Assert.AreEqual(twitteruser.ListedCount, feed.User.ListedCount);
            Assert.AreEqual(twitteruser.NumberOfFavorites, feed.User.NumberOfFavorites);
            Assert.AreEqual(twitteruser.NumberOfFollowers, feed.User.NumberOfFollowers);
            Assert.AreEqual(twitteruser.NumberOfFriends, feed.User.NumberOfFriends);
            Assert.AreEqual(twitteruser.NumberOfStatuses, feed.User.NumberOfStatuses);

            Assert.AreEqual(twitteruser.ScreenName, feed.User.ScreenName);
            Assert.AreEqual(twitteruser.StringId, feed.User.StringId);
            Assert.AreEqual(twitteruser.TimeZone, feed.User.TimeZone);
            Assert.AreEqual(twitteruser.Website, feed.User.Website);

        }
    }
}
