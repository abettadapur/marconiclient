using MarconiClient.Net;
using MarconiClient.V1_1;
using MarconiClient.V1_1.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    public class ClientTest
    {
        [TestMethod]
        public async Task createQueue()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.put("http://localhost:200/v1/queues/newQueue", "")).Returns(Task.FromResult(response));

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
