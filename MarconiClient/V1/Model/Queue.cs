
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarconiClient.Util;
using MarconiClient.Net;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("MarconiUnitTest")]
#endif

namespace MarconiClient.V1.Model
{
    /// <summary>
    /// The Queue object contains all the methods that can be performed on queues, including message reading, creation, and claiming.
    /// </summary>
    public class Queue
    {
        private string _name;
        private string _uri;
        private int ExistCheck = 0;
        private bool Exists = false;
        public string Name { get { return _name; } }
        public string Uri { get { return _uri; } }

        private QueueStats _Stats;
        //public bool Exists { get; set; }
        public QueueStats Stats { get { return _Stats; } }
        private IRequest request;


        /// <summary>
        /// Initializes a new instance of the <see cref="Queue"/> class. 
        /// </summary>
        /// <param name="name">Queue name.</param>
        /// <param name="uri">Location of queue.</param>
        /// <param name="request">Request object</param>
        internal Queue(string name, string uri, IRequest request)
        {
            _name = name;
            _uri = uri;
            this.request = request;
            updateStats();
            //Exists = true;

        }

        /// <summary>
        /// Updates the statistics for the queue
        /// </summary>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task updateStats()
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.get(Uri + "/stats");
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response);
                string json = await response.Content.ReadAsStringAsync();
                _Stats = JsonConvert.DeserializeObject<QueueStats>(json);
            }
            else
            {
                throw new QueueMissingException();
            }
        }
        public async Task setMetadata(object metadata)
        {
            if (await CheckExist())
            {
                string json = JsonConvert.SerializeObject(metadata);
                HttpResponseMessage response = await request.post(Uri + "/metadata", json);
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response); 
            }
            else
            {
                throw new QueueMissingException();
            }
        }
        /// <summary>
        /// Gets the metadata for the queue.
        /// </summary>
        /// <returns>A json string with the metadata</returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task<string> getMetadata()
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.get(Uri + "/metadata");
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response); 
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new QueueMissingException();
            }
        }

        /// <summary>
        /// Posts a new message to the queue.
        /// </summary>
        /// <param name="message">A message to post</param>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task postMessage(Message message)
        {
            if (await CheckExist())
            {
                List<Message> messages = new List<Message>();
                messages.Add(message);
                await postMessage(messages);
            }
            else
                throw new QueueMissingException();
        }

        /// <summary>
        /// Posts a list of new messages to the queue
        /// </summary>
        /// <param name="messages">Messages to post</param>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task postMessage(List<Message> messages)
        {
            if (await CheckExist())
            {
                string postbody = JsonConvert.SerializeObject(messages);
                HttpResponseMessage response = await request.post(Uri + "/messages", postbody);
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response);
                string json = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine(json);
                JObject jsonObj = JObject.Parse(json);
                int counter = 0;
                foreach (var item in jsonObj["resources"])
                {
                    Message m = messages[counter++];
                    string strRegex = @"^/[a-z0-9]*/queues/[a-zA-Z0-9]*/messages/([a-zA-Z0-9]*)";
                    Regex myRegex = new Regex(strRegex, RegexOptions.None);
                    string strTargetString = @"" + item;
                    m.ID = myRegex.Split(strTargetString)[1];
                }


            }
            else
                throw new QueueMissingException();
            
        }
       /* public async Task<List<Message>> getAllMessages()
        {
            HttpResponseMessage response = await request.get(Uri + "/messages?echo=true");
            response.EnsureSuccessStatusCode();
            JObject jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());
            List<Message> messages = new List<Message>();
            foreach(JObject message in jsonObj["messages"])
            {
                messages.Add(Message.Create(message));
            }
            return messages;

        }*/

        /// <summary>
        /// Gets one message from the queue by the message ID. Throws HttpException if the message does not exist
        /// </summary>
        /// <param name="ID">The identifier of the message</param>
        /// <returns>The Message object</returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task<Message> getMessage(string ID)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.get(Uri + "/messages/" + ID);
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response); 
                JObject jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());
                return Message.Create(jsonObj);
            }
            else
                throw new QueueMissingException();

        }

        /// <summary>
        /// Gets a series of messages from the queue by their IDs. Throws HttpException if the messages do not exist
        /// </summary>
        /// <param name="ID">Id of the first message</param>
        /// <param name="values">Ids of remaining messages</param>
        /// <returns>A list of Message objects</returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task<List<Message>> getMessages(string ID, params string[] values)
        {
            if (await CheckExist())
            {
                StringBuilder parameters = new StringBuilder("?ids=" + ID);
                foreach (string id in values)
                {
                    parameters.Append("," + id);
                }
                HttpResponseMessage response = await request.get(Uri + "/messages" + parameters.ToString());
                response.EnsureSuccessStatusCode();
                List<Message> messages = new List<Message>();

                JArray jsonArr = JArray.Parse(await response.Content.ReadAsStringAsync());
                foreach (JObject message in jsonArr)
                {
                    messages.Add(Message.Create(message));
                }
                return messages;
            }
            else
                throw new QueueMissingException();
        }

        /// <summary>
        /// Gets messages from the queue.
        /// </summary>
        /// <param name="marker">An opaque string that identifies where to start getting from. If blank, starts from beginning. This operation returns 
        /// a new marker to use in subsequent requests.</param>
        /// <param name="limit">How many messages to get</param>
        /// <param name="echo">if set to <c>true</c>, return messages that this client has posted</param>
        /// <param name="include_claimed">if set to <c>true</c> return already claimed messages.</param>
        /// <returns>A tuple containing the message objects and a new marker to use in subsequent requests</returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task<Tuple<List<Message>, string>> getMessages(string marker, int limit, bool echo, bool include_claimed)
        {
            if (await CheckExist())
            {
                StringBuilder parameters = new StringBuilder("?");
                if (marker != null && marker != "")
                    parameters.Append("marker=" + marker + "&");
                parameters.Append("limit=" + limit + "&");
                parameters.Append("echo=" + echo + "&");
                parameters.Append("include_claimed=" + include_claimed);

                HttpResponseMessage response = await request.get(Uri + "/messages" + parameters.ToString());
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response); 
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    List<Message> messages = new List<Message>();
                    JObject jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());
                    foreach (JObject message in jsonObj["messages"])
                    {
                        messages.Add(Message.Create(message));
                    }

                    string linkhref = jsonObj["links"][0]["href"].ToString();
                    var parsedQuery = HttpUtility.ParseQueryString(linkhref.Split('?')[1]);
                    string newmarker = parsedQuery["marker"];
                    //string strRegex = @"/[a-zA-Z0-9\.]*/queues/[a-zA-Z0-9\-_]*/messages\?marker=([0-9a-zA-Z\-_]*)";
                    //Regex myRegex = new Regex(strRegex, RegexOptions.None);
                    //string newmarker = myRegex.Split(linkhref)[1];


                    return Tuple.Create<List<Message>, string>(messages, newmarker);
                }
                else
                    return Tuple.Create<List<Message>, string>(new List<Message>(), "");
            }
            else
                throw new QueueMissingException();
        }

        /// <summary>
        /// Checks if the queue exists. 
        /// </summary>
        /// <returns>Does the queue exist?</returns>
        internal async Task<bool> CheckExist()
        {
            if (ExistCheck == 0)
            {
                HttpResponseMessage response = await request.get(Uri);
                Exists = response.IsSuccessStatusCode;
            }
            ExistCheck++;
            if (ExistCheck > 20)
                ExistCheck = 0;
            return Exists;
        }

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="ID">The id of the message</param>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task deleteMessage(string ID)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.delete(Uri + "/messages/" + ID);
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response);
            }
            else
                throw new QueueMissingException();
        }

        /// <summary>
        /// Deletes a claimed message. Claim ID is required, or the operation will fail
        /// </summary>
        /// <param name="ID">The id of the message</param>
        /// <param name="claimID">The claim id</param>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task deleteMessage(string ID, string claimID)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.delete(Uri + "/messages/" + ID + "?claim_id=" + claimID);
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response);
            }
            else
                throw new QueueMissingException();
        }

        /// <summary>
        /// Deletes a series of messages
        /// </summary>
        /// <param name="ID">The id for the first message</param>
        /// <param name="values">Ids for other messages</param>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task deleteMessages(string ID, params string[] values)
        {
            if (await CheckExist())
            {
                StringBuilder parameters = new StringBuilder("?ids=" + ID);
                foreach (string id in values)
                {
                    parameters.Append("," + id);
                }
                HttpResponseMessage response = await request.delete(Uri + "/messages" + parameters.ToString());
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response);
            }
            else
                throw new QueueMissingException();
        }
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Deletes the queue. Subsequent operations will throw QueueMissingExceptions
        /// </summary>
        /// <returns></returns>
        public async Task delete()
        {
            await request.delete(Uri);
        }

        /// <summary>
        /// Creates a new claim with a default limit of 10
        /// </summary>
        /// <param name="ttl">The TTL of the claim. Once this and the grace expire, the claim will automatically release</param>
        /// <param name="grace">The grace period of the claim</param>
        /// <returns>A list of claim objects that contain messages</returns>
        public async Task<List<Claim>> claim(int ttl, int grace)
        {
            return await claim(ttl, grace, 10);
        }

        /// <summary>
        /// Creates a new claim
        /// </summary>
        /// <param name="ttl">The TTL of the claim. Once this and the grace expire, the claim will automatically release.</param>
        /// <param name="grace">The grace period of the claim.</param>
        /// <param name="limit">How many messages to claim.</param>
        /// <returns>A list of claim objects that contain messages</returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task<List<Claim>> claim(int ttl, int grace, int limit)
        {
            if (await CheckExist())
            {
                if (limit <= 0)
                    return null;
                Dictionary<string, int> claimPostBody = new Dictionary<string, int>();
                claimPostBody["ttl"] = ttl;
                claimPostBody["grace"] = grace;
                string jsonbody = JsonConvert.SerializeObject(claimPostBody);
                HttpResponseMessage response = await request.post(Uri + "/claims?limit=" + limit, jsonbody);
                if (!response.IsSuccessStatusCode)
                     await Util.Util.throwException(response); 
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray jsonArr = JArray.Parse(json);
                    List<Claim> claims = new List<Claim>();
                    foreach (JObject jsonObj in jsonArr)
                    {
                        claims.Add(Claim.Create(jsonObj));
                    }
                    return claims;
                }
                return null;
            }
            else
                throw new QueueMissingException();
            
        }
        public async Task<Claim> getClaim(string claimid)
        {
            return null;  ///needs some thought
        }

        /// <summary>
        /// Updates the claim with a new TTL.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <param name="ttl">The new TTL.</param>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task patchClaim(string claimid, int ttl)
        {
            if (await CheckExist())
            {
                Dictionary<string, int> claimBody = new Dictionary<string, int>();
                claimBody.Add("ttl", ttl);
                HttpResponseMessage response = await request.patch(Uri + "/claims/" + claimid, JsonConvert.SerializeObject(claimBody));
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response); 
            }
            else
                throw new QueueMissingException();
        }

        /// <summary>
        /// Releases the claim. Other clients can now claim the messages
        /// </summary>
        /// <param name="claimid">The claimid to release</param>
        /// <returns></returns>
        /// <exception cref="QueueMissingException"></exception>
        public async Task releaseClaim(string claimid)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.delete(Uri + "/claims/" + claimid);
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response); 
            }
            else
                throw new QueueMissingException();

        }
  

    }   
    
}
