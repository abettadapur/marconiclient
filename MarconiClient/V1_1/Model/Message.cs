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
    /// <summary>
    /// Object that represents a message that can be posted to a queue
    /// </summary>
    public class Message
    {
        private int _ttl;
        private string _body;
        private string _id;
        private int _age;
        [JsonProperty("ttl")]

        /// <summary>
        /// Gets or sets the TTL
        /// </summary>
        /// <value>
        /// Specifies how long the message should exist before it is deleted
        /// </value>
        public int TTL { get { return _ttl; } set { _ttl = value; } }


        /// <summary>
        /// Gets the body of the message as a JSON string
        /// </summary>
        /// <value>
        /// The body. It is represented as a JSON string
        /// </value>
        [JsonConverter(typeof(RawJsonConverter))]
        [JsonProperty("body")]
        public string Body { get { return _body; } }


        /// <summary>
        /// Gets the id of the message.
        /// </summary>
        /// <value>
        /// The id of the message.
        /// </value>
        [JsonIgnore]
        public string ID { get { return _id; } set { _id = value; } }


        /// <summary>
        /// Gets the age of the message.
        /// </summary>
        /// <value>
        /// An integer that tells how long the message has existed
        /// </value>
        [JsonProperty("age")]
        [JsonIgnore]
        public int Age { get { return _age; } }



        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class. Default 60 ttl and null body
        /// </summary>
        public Message() : this(60, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class. Default null body
        /// </summary>
        /// <param name="ttl">The TTL for the message</param>
        public Message(int ttl) :this(ttl, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class. Default 60 ttl
        /// </summary>
        /// <param name="body">The body of the message</param>
        public Message(object body) : this(60, body)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="ttl">The TTL of the message</param>
        /// <param name="body">The body of the message</param>
        public Message(int ttl, object body)
        {
            this._ttl = ttl;
            this._body = JsonConvert.SerializeObject(body);
        }

        /// <summary>
        /// Creates a message from a json object
        /// </summary>
        /// <param name="jsonObj">The json object to create a message from</param>
        /// <returns>The created message</returns>
        public static Message Create(JObject jsonObj)
        {
            Message messageObj = new Message();
            int age, ttl;
            int.TryParse(jsonObj["age"].ToString(), out age);
            int.TryParse(jsonObj["ttl"].ToString(), out ttl);
            messageObj._age = age;
            messageObj.TTL = ttl;
            messageObj._id = jsonObj["id"].ToString();
            //string strRegex = @"^/[a-z0-9]*/queues/[a-zA-Z0-9\-]*/messages/([a-zA-Z0-9]*)";
            /*Regex myRegex = new Regex(strRegex, RegexOptions.None);
            string strTargetString = @"" + jsonObj["href"].ToString();
            messageObj.ID = myRegex.Split(strTargetString)[1];*/
            messageObj._body = jsonObj["body"].ToString();
            return messageObj;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _body;
        }
    }
}
