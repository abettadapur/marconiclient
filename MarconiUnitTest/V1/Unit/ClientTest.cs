using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MarconiClient.Net;
using System.Net.Http;
using System.Net;
using MarconiClient.V1;
using MarconiClient.V1.Model;
using Newtonsoft.Json;

namespace MarconiUnitTest.Unit.V1
{
    [TestClass]
    public class ClientTestV1
    {
        [TestMethod]
        public async Task createQueue()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.put("http://localhost:200/v1/queues/newQueue","")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1", mock.Object);
            Queue queue = await client.createQueue("newQueue");

            Assert.AreEqual("newQueue", queue.Name);
            Assert.AreEqual("http://localhost:200/v1/queues/newQueue", queue.Uri);

            mock.Verify(foo => foo.put("http://localhost:200/v1/queues/newQueue", ""), Times.Once);

        }


        [TestMethod]
        public async Task getQueues()
        {
            //create dictionary. {links:[{rel, href}], queues[{name, href, metadata{}},{},{}]}
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            Dictionary<string, object> responsebody = new Dictionary<string,object>()
            {
                {
                    "links", new List<Dictionary<string,object>>()
                    {
                        new Dictionary<string, object>()
                        {
                            {"rel", "next"},
                            {"href", "/v1/queues?marker=kooleo&limit=10&detailed=true"}
                        }
                    }
                },
                {
                    "queues", new List<Dictionary<string, object>>()
                    {
                        new Dictionary<string, object>()
                        {
                            {"name","boomerang"},
                            {"href", "/v1/queues/boomerang"}
                        },
                        new Dictionary<string, object>()
                        {
                            {"name", "foo"},
                            {"href", "/v1/queues/foo"}
                        }
                    }
                }
            };

            string jsonstr = JsonConvert.SerializeObject(responsebody);
            response.Content = new StringContent(jsonstr);
            
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1", mock.Object);
            List<Queue> queues = await client.getQueues();

            Assert.AreEqual("boomerang", queues[0].Name);
            Assert.AreEqual("http://localhost:200/v1/queues/boomerang", queues[0].Uri);
            Assert.AreEqual("foo", queues[1].Name);
            Assert.AreEqual("http://localhost:200/v1/queues/foo", queues[1].Uri);

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
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1", mock.Object);
            List<Queue> queues = await client.getQueues();

            Assert.AreEqual(0, queues.Count);
        }

        [TestMethod]
        public async Task isQueueCreated()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Client client = new Client("http://localhost", 200, "v1", mock.Object);
            bool exists = await client.isQueueCreated("newQueue");

            Assert.IsTrue(exists);
   
            mock.Verify(foo => foo.get("http://localhost:200/v1/queues/newQueue"), Times.Once);
        }

      
    }
}
