using MarconiClient.Net;
using MarconiClient.V1_1;
using MarconiClient.V1_1.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarconiUnitTest.V1_1
{
    [TestClass]
    public class ClientTestV1_1
    {
        [TestMethod]
        public async Task createQueue()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.put("http://localhost:200/v1.1/queues/newQueue", "")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1.1", mock.Object);
            Queue queue = await client.createQueue("newQueue");

            Assert.AreEqual("newQueue", queue.Name);
            Assert.AreEqual("http://localhost:200/v1.1/queues/newQueue", queue.Uri);

            mock.Verify(foo => foo.put("http://localhost:200/v1.1/queues/newQueue", ""), Times.Once);

        }

        [TestMethod]
        public async Task getQueues()
        {
            //create dictionary. {links:[{rel, href}], queues[{name, href, metadata{}},{},{}]}
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            Dictionary<string, object> responsebody = new Dictionary<string, object>()
            {
                {
                    "links", new List<Dictionary<string,object>>()
                    {
                        new Dictionary<string, object>()
                        {
                            {"rel", "next"},
                            {"href", "queues?marker=kooleo&limit=10&detailed=true"}
                        }
                    }
                },
                {
                    "queues", new List<Dictionary<string, object>>()
                    {
                        new Dictionary<string, object>()
                        {
                            {"name","boomerang"},
                            {"href", "queues/boomerang"}
                        },
                        new Dictionary<string, object>()
                        {
                            {"name", "foo"},
                            {"href", "queues/foo"}
                        }
                    }
                }
            };

            string jsonstr = JsonConvert.SerializeObject(responsebody);
            response.Content = new StringContent(jsonstr);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1.1/queues")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1.1", mock.Object);
            List<Queue> queues = await client.getQueues();

            Assert.AreEqual("boomerang", queues[0].Name);
            Assert.AreEqual("http://localhost:200/v1.1/queues/boomerang", queues[0].Uri);
            Assert.AreEqual("foo", queues[1].Name);
            Assert.AreEqual("http://localhost:200/v1.1/queues/foo", queues[1].Uri);
        }

        [TestMethod]
        public async Task getQueuesEmpty()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            Dictionary<string, object> responsebody = new Dictionary<string, object>()
            {
                {
                    "links", new List<Dictionary<string,object>>()
                    {
                        new Dictionary<string, object>()
                        {
                            {"rel", "next"},
                            {"href", "queues?marker=kooleo&limit=10&detailed=true"}
                        }
                    }
                },
                {
                    "queues", new List<Dictionary<string, object>>()
                    {
                       
                    }
                }
            };

            string jsonstr = JsonConvert.SerializeObject(responsebody);
            response.Content = new StringContent(jsonstr);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1.1/queues")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1.1", mock.Object);
            List<Queue> queues = await client.getQueues();

            Assert.AreEqual(0, queues.Count);
        }

        [TestMethod]
        public async Task isQueueCreated()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1.1/queues/newQueue")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1.1", mock.Object);
            bool exists = await client.isQueueCreated("newQueue");

            Assert.IsTrue(exists);

            mock.Verify(foo => foo.get("http://localhost:200/v1.1/queues/newQueue"), Times.Once);
        }

        [TestMethod]
        public async Task createShard()
        {

            Dictionary<string, object> body = new Dictionary<string, object>();
            body["uri"] = "http://localhost:27017";
            body["weight"] = 100;
          
            string bodystr = JsonConvert.SerializeObject(body);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.put("http://localhost:200/v1.1/shards/newShard", bodystr)).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1.1", mock.Object);

            Shard s = await client.createShard("newShard", "http://localhost:27017", 100, null);
            Assert.AreEqual("newShard", s.Name);
            Assert.AreEqual(100, s.Weight);
            Assert.AreEqual("http://localhost:27017", s.Uri);

            mock.Verify(foo => foo.put("http://localhost:200/v1.1/shards/newShard", bodystr), Times.Once);

        }

        [TestMethod]
        public async Task getShards()
        {
            List<Dictionary<string, object>> responsebody = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"href", "/v1.1/shards/wat"},
                    {"weight", 100},
                    {"uri", "mongodb://marconi1.example.com:27017"},
                    {"options", new Dictionary<string, object>()}
                },

                new Dictionary<string, object>()
                {
                    {"href", "/v1.1/shards/waht"},
                    {"weight", 100},
                    {"uri", "mongodb://marconi1.example.com:27018"}                ,
                    {"options", new Dictionary<string, object>()}
                },

            };


            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            string jsonstr = JsonConvert.SerializeObject(responsebody);
            response.Content = new StringContent(jsonstr);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1.1/shards/?limit=10&detailed=true")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1.1", mock.Object);

            List<Shard> shards = await client.getShards(10);
            Assert.AreEqual(2, shards.Count);
            Assert.AreEqual("mongodb://marconi1.example.com:27017", shards[0].Uri);
            Assert.AreEqual("mongodb://marconi1.example.com:27018", shards[1].Uri);

            mock.Verify(foo => foo.get("http://localhost:200/v1.1/shards/?limit=10&detailed=true"), Times.Once);
        }
    }
}
