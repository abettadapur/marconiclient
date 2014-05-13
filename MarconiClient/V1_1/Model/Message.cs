using MarconiClient.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MarconiClient.V1_1.Model
{
    public class Message
    {
        private int _ttl;
        private string _body;
        [JsonProperty("ttl")]
        public int TTL { get { return _ttl; } set { _ttl = value; } }

        [JsonConverter(typeof(RawJsonConverter))]
        [JsonProperty("body")]
        public string Body { get { return _body; } set { _body = value; } }

        [JsonIgnore]
        public string ID { get; set; }

        [JsonProperty("age")]
        [JsonIgnore]
        public int Age { get; set; }



        public Message() : this(60, null)
        {
        }
        public Message(int ttl) :this(ttl, null)
        {

        }
        public Message(object body) : this(60, body)
        {

        }
        public Message(int ttl, object body)
        {
            this._ttl = ttl;
            this._body = JsonConvert.SerializeObject(body);
        }
        public static Message Create(JObject jsonObj)
        {
            Message messageObj = new Message();
            int age, ttl;
            int.TryParse(jsonObj["age"].ToString(), out age);
            int.TryParse(jsonObj["ttl"].ToString(), out ttl);
            messageObj.Age = age;
            messageObj.TTL = ttl;
            string strRegex = @"^/[a-z0-9]*/queues/[a-zA-Z0-9\-]*/messages/([a-zA-Z0-9]*)";
            Regex myRegex = new Regex(strRegex, RegexOptions.None);
            string strTargetString = @"" + jsonObj["href"].ToString();
            messageObj.ID = myRegex.Split(strTargetString)[1];
            messageObj.Body = jsonObj["body"].ToString();
            return messageObj;
        }
        public override string ToString()
        {
            return _body;
        }
    }
}
