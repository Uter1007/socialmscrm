using System;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitterizer;
using LinqToTwitter;

namespace uter.sociallistener.tests.MapperTest
{
    [TestClass]
    public class TwitterUserMapperTests
    {

        [TestMethod]
        public void TwitterLinqUserMapTest()
        {

            var twitteruser = new User()
            {
                ID = "123",
                CreatedAt = new DateTime(2012, 1, 1),
                Description = "Test",
                Notifications = false,
                FollowRequestSent = false,
                ContributorsEnabled = true,
                Following = true,
                GeoEnabled = false,
                Protected = false,
                Lang = "German",
                ListedCount = 0,
                FavoritesCount = 0,
                FollowersCount = 2,
                FriendsCount = 3,
                StatusesCount = 10,
                ScreenName = "Testuser",
                TimeZone = "GMT+1",
                Url = "www.orf.at"
            };


            var tweetprofile = TwitterUserMapper.Map(twitteruser);

            Assert.AreEqual(twitteruser.ID, tweetprofile.Id.ToString());
            Assert.AreEqual(twitteruser.CreatedAt, tweetprofile.CreatedDate);
            Assert.AreEqual(twitteruser.Description, tweetprofile.Description);
            Assert.AreEqual(twitteruser.Notifications, tweetprofile.DoesReceiveNotifications);
            //Assert.AreEqual(twitteruser.FollowRequestSent, tweetprofile.FollowRequestSent);
            Assert.AreEqual(twitteruser.ContributorsEnabled, tweetprofile.IsContributorsEnabled);
            //Assert.AreEqual(twitteruser.IsFollowedBy, tweetprofile.IsFollowedBy);
            Assert.AreEqual(twitteruser.Following, tweetprofile.IsFollowing);

            Assert.AreEqual(twitteruser.GeoEnabled, tweetprofile.IsGeoEnabled);
            Assert.AreEqual(twitteruser.Protected, tweetprofile.IsProtected);
            Assert.AreEqual(twitteruser.Lang, tweetprofile.Language);
            Assert.AreEqual(twitteruser.ListedCount, tweetprofile.ListedCount);
            Assert.AreEqual(twitteruser.FavoritesCount, tweetprofile.NumberOfFavorites);
            Assert.AreEqual(twitteruser.FollowersCount, tweetprofile.NumberOfFollowers);
            Assert.AreEqual(twitteruser.FriendsCount, tweetprofile.NumberOfFriends);
            Assert.AreEqual(twitteruser.StatusesCount, tweetprofile.NumberOfStatuses);

            Assert.AreEqual(twitteruser.ScreenName, tweetprofile.ScreenName);
            Assert.AreEqual(twitteruser.ID, tweetprofile.StringId);
            Assert.AreEqual(twitteruser.TimeZone, tweetprofile.TimeZone);
            Assert.AreEqual(twitteruser.UtcOffset, tweetprofile.TimeZoneOffset);
            Assert.AreEqual(twitteruser.Url, tweetprofile.Website);
        }


        [TestMethod]
        public void TwitterizerUserMapTest()
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

            var tweetprofile = TwitterUserMapper.Map(twitteruser);

            Assert.AreEqual(twitteruser.Id, tweetprofile.Id);
            Assert.AreEqual(twitteruser.CreatedDate, tweetprofile.CreatedDate);
            Assert.AreEqual(twitteruser.Description, tweetprofile.Description);
            Assert.AreEqual(twitteruser.DoesReceiveNotifications, tweetprofile.DoesReceiveNotifications);
            //Assert.AreEqual(twitteruser.FollowRequestSent, tweetprofile.FollowRequestSent);
            Assert.AreEqual(twitteruser.IsContributorsEnabled, tweetprofile.IsContributorsEnabled);
            Assert.AreEqual(twitteruser.IsFollowedBy, tweetprofile.IsFollowedBy);
            Assert.AreEqual(twitteruser.IsFollowing, tweetprofile.IsFollowing);

            Assert.AreEqual(twitteruser.IsGeoEnabled, tweetprofile.IsGeoEnabled);
            Assert.AreEqual(twitteruser.IsProtected, tweetprofile.IsProtected);
            Assert.AreEqual(twitteruser.Language, tweetprofile.Language);
            Assert.AreEqual(twitteruser.ListedCount, tweetprofile.ListedCount);
            Assert.AreEqual(twitteruser.NumberOfFavorites, tweetprofile.NumberOfFavorites);
            Assert.AreEqual(twitteruser.NumberOfFollowers, tweetprofile.NumberOfFollowers);
            Assert.AreEqual(twitteruser.NumberOfFriends, tweetprofile.NumberOfFriends);
            Assert.AreEqual(twitteruser.NumberOfStatuses, tweetprofile.NumberOfStatuses);

            Assert.AreEqual(twitteruser.ScreenName, tweetprofile.ScreenName);
            Assert.AreEqual(twitteruser.StringId, tweetprofile.StringId);
            Assert.AreEqual(twitteruser.TimeZone, tweetprofile.TimeZone);
            Assert.AreEqual(twitteruser.Website, tweetprofile.Website);

        }
    }
}
