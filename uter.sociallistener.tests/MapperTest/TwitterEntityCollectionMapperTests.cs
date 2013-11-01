using System;
using uter.sociallistener.general.Twitter.Models;
using uter.sociallistener.general.Twitter.Models.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitterizer.Entities;

namespace uter.sociallistener.tests.MapperTest
{
    [TestClass]
    public class TwitterEntityCollectionMapperTests
    {
        [TestMethod]
        public void TwitterEntityCollectionMapperTest()
        {
            var entitycollection = new TwitterEntityCollection() { 
                new TwitterHashTagEntity() { Text = "hashtag" }, 
                new TwitterUrlEntity() { Url = "www.orf.at" } 
            };

            var list = TwitterEntityCollectionMapper.Map(entitycollection);

            Assert.AreEqual(entitycollection.Count, list.Count);
            Assert.AreEqual(((TwitterHashTagEntity)entitycollection[0]).Text, ((TwitterHashTag)list[0]).Text);
        }
    }
}
