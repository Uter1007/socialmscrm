using System;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitterizer.Entities;

namespace uter.sociallistener.tests.MapperTest
{
    [TestClass]
    public class TwitterEntitiesMapperTests
    {
        [TestMethod]
        public void TwitterHashTagMapTest()
        {
            var twitterhashtag = new TwitterHashTagEntity() { Text = "testhashtag" };

            var hashtag = TwitterHashTagMapper.Map(twitterhashtag);

            Assert.AreEqual(twitterhashtag.Text, hashtag.Text);
        }

        [TestMethod]
        public void TwitterUrlMapTest()
        {
            var twitterurl = new TwitterUrlEntity() { Url = "www.orf.at" };

            var url = TwitterUrlMapper.Map(twitterurl);

            Assert.AreEqual(twitterurl.Url, url.Url);
        }

        [TestMethod]
        public void TwitterMentionMapTest()
        {
            var twitteruser = new TwitterMentionEntity() { Name = "test", ScreenName = "test2", UserId = (decimal)123 };

            var user = TwitterMentionMapper.Map(twitteruser);

            Assert.AreEqual(twitteruser.UserId, user.UserId);
            Assert.AreEqual(twitteruser.ScreenName, user.ScreenName);
            Assert.AreEqual(twitteruser.Name, user.Name);
        }

      
    }
}
