using System;
using uter.sociallistener.general.CRM.Models;
using uter.sociallistener.general.CRM.Models.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace uter.sociallistener.tests.MapperTest
{
    [TestClass]
    public class TwitterConfigMapperTests
    {
        [TestMethod]
        public void TwitterConfigMapperTest()
        {
            var entity = new Entity("cott_twitterconfig");
            var guid = Guid.NewGuid();
            entity.Id = guid;

            entity.Attributes.Add("cott_location", "testcity");
            entity.Attributes.Add("cott_website", "www.google.com");
            entity.Attributes.Add("cott_personalname", "testname");
            entity.Attributes.Add("cott_twitterpic", "http://www.tweetpic.com");
            entity.Attributes.Add("cott_accesstoken", "wedwidmwemd");
            entity.Attributes.Add("cott_accesstokensecret", "wewewefmwemfwe");
            entity.Attributes.Add("cott_lastid", "4234234");
            entity.Attributes.Add("cott_count", 123);

            var config = CRMTwitterConfigMapper.Map(entity);

            Assert.AreEqual(entity.Id, config.CRMID);
            Assert.AreEqual((string)entity["cott_location"], config.Location);
            Assert.AreEqual((string)entity["cott_website"], config.Website);
            Assert.AreEqual((string)entity["cott_personalname"], config.PersonalName);
            Assert.AreEqual((string)entity["cott_twitterpic"], config.TwitterPic);
            Assert.AreEqual((string)entity["cott_accesstoken"], config.Accesstoken);
            Assert.AreEqual((string)entity["cott_accesstokensecret"], config.Accesstokensecret);
            Assert.AreEqual((string)entity["cott_lastid"], config.LastStatusId);
            Assert.AreEqual((int)entity["cott_count"], config.MaxRecords);
        }

        [TestMethod]
        public void TwitterConfigMapperReverseTest()
        {
            var config = new TwitterConfig();
            var guid = Guid.NewGuid();
            config.CRMID = guid;
            config.Accesstoken = "213123";
            config.Accesstokensecret = "dewinfewn";
            config.LastStatusId = "123123";
            config.TwitterPic = "http://dwem";
            config.Location = "dwefewf";
            config.Website = "diwenifnewifn";
            config.PersonalName = "ewinfiewnfin";
            config.MaxRecords = 123;

            var entity = CRMTwitterConfigMapper.Map(config);

            Assert.AreEqual(entity.Id, config.CRMID);
            Assert.AreEqual((string)entity["cott_location"], config.Location);
            Assert.AreEqual((string)entity["cott_website"], config.Website);
            Assert.AreEqual((string)entity["cott_personalname"], config.PersonalName);
            Assert.AreEqual((string)entity["cott_twitterpic"], config.TwitterPic);
            Assert.AreEqual((string)entity["cott_accesstoken"], config.Accesstoken);
            Assert.AreEqual((string)entity["cott_accesstokensecret"], config.Accesstokensecret);
            Assert.AreEqual((string)entity["cott_lastid"], config.LastStatusId);
            Assert.AreEqual((int)entity["cott_count"], config.MaxRecords);

        }
    }
}
