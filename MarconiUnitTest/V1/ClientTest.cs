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

namespace MarconiUnitTest.V1
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
