using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.CRM.Models.Mapping;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace uter.sociallistener.tests.MapperTest
{
    [TestClass]
    public class CRMTwitterEntityMapper
    {
        [TestMethod]
        public void CRMHashTagMapperTest()
        {
            var guid = Guid.NewGuid();
            var hash = new TwitterHashTag(){CRMID = guid, Text = "dnwenfe"};

            var configid = Guid.NewGuid();
            var entity = TwitterHashTagMapper.Map(hash,configid);

            Assert.AreEqual(entity.Id, hash.CRMID);
            Assert.AreEqual((string)entity["cott_name"], hash.Text);
         
        }

        [TestMethod]
        public void CRMUrlMapperTest()
        {
            var guid = Guid.NewGuid();
            var url = new TwitterUrl() { CRMID = guid, Url = "dnwenfe" };

            var configid = Guid.NewGuid();
            var entity = TwitterUrlMapper.Map(url, configid);

            Assert.AreEqual(entity.Id, url.CRMID);
            Assert.AreEqual((string)entity["cott_name"], url.Url);
        }

        [TestMethod]
        public void CRMMentionMapperTest()
        {
            var guid = Guid.NewGuid();
            var mention = new TwitterMention() { CRMID = guid, Name = "dnwenfe", ScreenName ="tede", UserId = (decimal)123213 };

            var configid = Guid.NewGuid();
            var entity = TwitterMentionMapper.Map(mention, configid);

            Assert.AreEqual(entity.Id, mention.CRMID);
            Assert.AreEqual((string)entity["cott_name"], mention.ScreenName);
            Assert.AreEqual((string)entity["cott_userid"], mention.UserId.ToString());
        }
    }
}
