using MarconiClient.Net;
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

namespace MarconiUnitTest.Unit.V1_1
{
    [TestClass]
    public class ShardTestV1_1
    {
        [TestMethod]
        public async Task changeUri()
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body["uri"] = "mongodb://127.0.0.1:27017";
            string jsonbody = JsonConvert.SerializeObject(body);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.patch("http://localhost:200/v1.1/shards/newShard", jsonbody )).Returns(Task.FromResult(response));

            Shard shard = new Shard("newShard", "mongodb://127.0.0.1:27018", 100, null, "http://localhost:200/v1.1/shards/newShard", mock.Object);
            await shard.changeUri("mongodb://127.0.0.1:27017");

            mock.Verify(foo => foo.patch("http://localhost:200/v1.1/shards/newShard", jsonbody), Times.Once);

        }

        [TestMethod]
        public async Task changeWeight()
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body["weight"] = 10;
            string jsonbody = JsonConvert.SerializeObject(body);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.patch("http://localhost:200/v1.1/shards/newShard", jsonbody)).Returns(Task.FromResult(response));

            Shard shard = new Shard("newShard", "mongodb://127.0.0.1:27018", 100, null, "http://localhost:200/v1.1/shards/newShard", mock.Object);
            await shard.changeWeight(10);

            mock.Verify(foo => foo.patch("http://localhost:200/v1.1/shards/newShard", jsonbody), Times.Once);
        }

        [TestMethod]
        public async Task changeOptions()
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body["weight"] = 10;
            string jsonbody = JsonConvert.SerializeObject(body);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.patch("http://localhost:200/v1.1/shards/newShard", jsonbody)).Returns(Task.FromResult(response));

            Shard shard = new Shard("newShard", "mongodb://127.0.0.1:27018", 100, null, "http://localhost:200/v1.1/shards/newShard", mock.Object);
            await shard.changeWeight(10);

            mock.Verify(foo => foo.patch("http://localhost:200/v1.1/shards/newShard", jsonbody), Times.Once);
        }

        [TestMethod]
        public async Task deleteShard()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            var mock = new Mock<IRequest>();
            mock.Setup(foo => foo.delete("http://localhost:200/v1.1/shards/newShard")).Returns(Task.FromResult(response));

            Shard shard = new Shard("newShard", "mongodb://127.0.0.1:27018", 100, null, "http://localhost:200/v1.1/shards/newShard", mock.Object);
            await shard.deleteShard();
            mock.Verify(foo => foo.delete("http://localhost:200/v1.1/shards/newShard"), Times.Once);
        }
    }
}
