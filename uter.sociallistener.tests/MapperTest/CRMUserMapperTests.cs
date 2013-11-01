using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uter.sociallistener.general.CRM.Models;
using uter.sociallistener.general.CRM.Models.Mapping;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace uter.sociallistener.tests.MapperTest
{
    [TestClass]
    public class CRMUserMapperTests
    {
        //[TestMethod]
        //public void CRMUserMapperTest()
        //{
        //    var entity = new Entity("cott_twitteruser");
        //    var guid = Guid.NewGuid();
        //    entity.Id = guid;
        //    entity.Attributes.Add("cott_name", "tewef");
        //    entity.Attributes.Add("cott_userid", "212312");
        //    entity.Attributes.Add("cott_twitterpic", "wiefwefin");

        //    var user = TwitterUserMapper.Map(entity);

            
        //    Assert.AreEqual((string)entity["cott_name"], user.Name);
        //    Assert.AreEqual((string)entity["cott_userid"], user.Id);
        //    Assert.AreEqual((string)entity["cott_twitterpic"], user.ProfileBackgroundImageLocation);


        //}

        //[TestMethod]
        //public void CRMUserMapperReverseTest()
        //{
        //    var guid = Guid.NewGuid();
        //    var configid = Guid.NewGuid();
        //    var user = new TwitterUserProfile() { Id = 123, Name = "ddfwe", ProfileBackgroundImageLocation = "wefew" };


        //    var entity = TwitterUserMapper.Map(user, configid);

            
        //    Assert.AreEqual((string)entity["cott_name"], user.Name);
        //    Assert.AreEqual((string)entity["cott_userid"], user.Id);
        //    Assert.AreEqual((string)entity["cott_twitterpic"], user.ProfileBackgroundImageLocation);
        //    Assert.AreEqual(((EntityReference)entity["cott_createdbyconfig"]).Id, configid);


        //}
    }
}
