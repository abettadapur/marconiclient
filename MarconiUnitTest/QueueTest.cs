using System;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarconiClient.V1.Model;
using MarconiClient.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodeGrooverClient.Model;
using Newtonsoft.Json;
using System.Web;
using MarconiClient.Util;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace MarconiUnitTest
{
    [TestClass]
    public class QueueTest
    {
        [TestMethod]
        public void updateStats()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            
            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object); //called from this object
            //queue.updateStats();

            mock.Verify(foo => foo.get("http://localhost:200/v1/queues/newQueue/stats"), Times.Once);
        }
        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public async Task updateStatsNegative()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            HttpResponseMessage badresponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(badresponse));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object); //called from this object
            await queue.updateStats();

            mock.Verify(foo => foo.get("http://localhost:200/v1/queues/newQueue/stats"), Times.Once);
        }

        [TestMethod]
        public async Task setMetadata()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.post("http://localhost:200/v1/queues/newQueue/metadata",It.IsAny<string>())).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.setMetadata("Metadata");

            mock.Verify(foo => foo.post("http://localhost:200/v1/queues/newQueue/metadata", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public async Task setMetadataNegative()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            HttpResponseMessage badresponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.post("http://localhost:200/v1/queues/newQueue/metadata", It.IsAny<string>())).Returns(Task.FromResult(badresponse));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.setMetadata("Metadata");

            mock.Verify(foo => foo.post("http://localhost:200/v1/queues/newQueue/metadata", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task getMetadata()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            Song data = new Song() { Name = "Song", ArtistName = "Artist" };
            string jsondata = JsonConvert.SerializeObject(data);
            response.Content = new StringContent(jsondata);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/metadata")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            string json = await queue.getMetadata();
            Assert.AreEqual(jsondata, json);
            mock.Verify(foo => foo.get("http://localhost:200/v1/queues/newQueue/metadata"), Times.Once);

        }
        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public async Task getMetadataNegative()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            HttpResponseMessage badresponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            Song data = new Song() { Name = "Song", ArtistName = "Artist" };
            string jsondata = JsonConvert.SerializeObject(data);
            response.Content = new StringContent(jsondata);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/metadata")).Returns(Task.FromResult(badresponse));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            string json = await queue.getMetadata();
            Assert.AreEqual(jsondata, json);
            mock.Verify(foo => foo.get("http://localhost:200/v1/queues/newQueue/metadata"), Times.Once);

        }

        [TestMethod]
        public async Task postSingleMessage()
        {
            Dictionary<string, object> responsebody = new Dictionary<string,object>();
            responsebody["partial"] = false;
            responsebody["resources"] = new string[]{"/v1/queues/newQueue/messages/31"};
            string responsestr = JsonConvert.SerializeObject(responsebody);

            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(responsestr);
  

            Message message = new Message(60, new Song() { Name = "Song", ArtistName = "Artist" });


            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.post("http://localhost:200/v1/queues/newQueue/messages", It.IsAny<string>())).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.postMessage(message);

            Assert.AreEqual(message.ID, "31");
            mock.Verify(foo => foo.post("http://localhost:200/v1/queues/newQueue/messages", It.IsAny<string>()), Times.Once);


        }
        [TestMethod]
        public async Task postMultipleMessages()
        {
            Dictionary<string, object> responsebody = new Dictionary<string, object>();
            responsebody["partial"] = false;
            responsebody["resources"] = new string[] { "/v1/queues/newQueue/messages/31", "/v1/queues/newQueue/messages/32" };
            string responsestr = JsonConvert.SerializeObject(responsebody);

            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(responsestr);


            Message message = new Message(60, new Song() { Name = "Song", ArtistName = "Artist" });
            Message message2 = new Message(60, new Song() { Name = "Song", ArtistName = "Artist" });
            List<Message> messages = new List<Message>() { message, message2 };


            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.post("http://localhost:200/v1/queues/newQueue/messages", It.IsAny<string>())).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.postMessage(messages);

            Assert.AreEqual(message.ID, "31");
            Assert.AreEqual(message2.ID, "32");
            mock.Verify(foo => foo.post("http://localhost:200/v1/queues/newQueue/messages", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task getMessage()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            Message message = new Message(60, new Song() { Name = "Song", ArtistName = "Artist" });

            JObject jsonobj = JObject.Parse(JsonConvert.SerializeObject(message));
            jsonobj["age"] = 60;
            jsonobj["href"] = "/v1/queues/newQueue/messages/31";
            string jsonstr = jsonobj.ToString();

            response.Content = new StringContent(jsonstr);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/messages/31")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue",mock.Object);
            Message m = await queue.getMessage("31");

            Assert.AreEqual(m.ID, "31");
            
            string originalbody = message.Body;
            string newbody = m.Body;
            originalbody = Regex.Replace(originalbody, @"\s", "");
            newbody = Regex.Replace(newbody, @"\s", "");

            Assert.IsTrue(newbody.Equals(originalbody, StringComparison.OrdinalIgnoreCase));
            mock.Verify(foo => foo.get("http://localhost:200/v1/queues/newQueue/messages/31"), Times.Once);

        }

        [TestMethod]
        public async Task getMessages()
        {

        }

        public async Task getMessagesMarker()
        {

        }

        [TestMethod]
        public async Task checkExist()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            Assert.IsTrue(await queue.CheckExist());
        }

        [TestMethod]
        public async Task deleteMessage()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo=> foo.delete("http://localhost:200/v1/queues/newQueue/messages/31")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.deleteMessage("31");
            mock.Verify(foo => foo.delete("http://localhost:200/v1/queues/newQueue/messages/31"), Times.Once);
        }
        [TestMethod]
        public async Task deleteMessages()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.delete("http://localhost:200/v1/queues/newQueue/messages?ids=31,32,33")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.deleteMessages("31", "32", "33");

            mock.Verify(foo => foo.delete("http://localhost:200/v1/queues/newQueue/messages?ids=31,32,33"), Times.Once);

        }

        [TestMethod]
        public async Task deleteMessageClaim()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.delete("http://localhost:200/v1/queues/newQueue/messages/31?claim_id=40")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.deleteMessage("31", "40");

            mock.Verify(foo => foo.delete("http://localhost:200/v1/queues/newQueue/messages/31?claim_id=40"), Times.Once);
        }

        [TestMethod]
        public async Task deleteQueue()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.delete("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.delete();

            mock.Verify(foo => foo.delete("http://localhost:200/v1/queues/newQueue"), Times.Once);
        }

        [TestMethod]
        public async Task claim()
        {
            Dictionary<string, int> claimPostBody = new Dictionary<string, int>();
            claimPostBody["ttl"] = 300;
            claimPostBody["grace"] = 300;
            string jsonbody = JsonConvert.SerializeObject(claimPostBody);

            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            List<Dictionary<string, object>> claimresponse = new List<Dictionary<string, object>>();
            Dictionary<string, object> firstClaim = new Dictionary<string, object>();
            firstClaim["href"] = "/v1/queues/foo-bar/messages/50b68a50d6f5b8c8a7c62b01?claim_id=a28ee94e-6cb4-11e2-b4d5-7703267a7926";
            firstClaim["ttl"] = 800;
            firstClaim["age"] = 100;
            firstClaim["body"] = "body";
            claimresponse.Add(firstClaim);
            Dictionary<string, object> secondClaim = new Dictionary<string, object>();
            secondClaim["href"] = "/v1/queues/foo-bar/messages/50b68a50d6f5b8c8a7c62b02?claim_id=a28ee94e-6cb4-11e2-b4d5-7703267a7926";
            secondClaim["ttl"] = 800;
            secondClaim["age"] = 790;
            secondClaim["body"] = "body2";
            claimresponse.Add(secondClaim);

            string responsestr = JsonConvert.SerializeObject(claimresponse);

            response.Content = new StringContent(responsestr);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.post("http://localhost:200/v1/queues/newQueue/claims?limit=10", jsonbody)).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            List<Claim> claims = await queue.claim(300, 300, 10);

            Assert.AreEqual("a28ee94e-6cb4-11e2-b4d5-7703267a7926", claims[0].ClaimID);
            Assert.AreEqual("a28ee94e-6cb4-11e2-b4d5-7703267a7926", claims[1].ClaimID);
            Assert.AreEqual("body", claims[0].Message.Body);
            Assert.AreEqual("body2", claims[1].Message.Body);
            Assert.AreEqual(800, claims[0].Message.TTL);
            Assert.AreEqual(790, claims[1].Message.Age);
            Assert.AreEqual("50b68a50d6f5b8c8a7c62b01", claims[0].Message.ID);
            Assert.AreEqual("50b68a50d6f5b8c8a7c62b02", claims[1].Message.ID);

            mock.Verify(foo => foo.post("http://localhost:200/v1/queues/newQueue/claims?limit=10", jsonbody), Times.Once);
        }

        [TestMethod]
        public async Task patchClaim()
        {
            Dictionary<string, int> claimBody = new Dictionary<string, int>();
            claimBody.Add("ttl", 300);
            string jsonbody = JsonConvert.SerializeObject(claimBody);

            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.patch("http://localhost:200/v1/queues/newQueue/claims/claimid31", jsonbody)).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.patchClaim("claimid31", 300);
            mock.Verify(foo => foo.patch("http://localhost:200/v1/queues/newQueue/claims/claimid31", jsonbody), Times.Once);

            
        }

        [TestMethod]
        public async Task releaseClaim()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue/status")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.get("http://localhost:200/v1/queues/newQueue")).Returns(Task.FromResult(response));
            mock.Setup(foo => foo.delete("http://localhost:200/v1/queues/newQueue/claims/claimid31")).Returns(Task.FromResult(response));

            Queue queue = new Queue("newQueue", "http://localhost:200/v1/queues/newQueue", mock.Object);
            await queue.releaseClaim("claimid31");
            mock.Verify(foo => foo.delete("http://localhost:200/v1/queues/newQueue/claims/claimid31"), Times.Once);

        }







    }
}
