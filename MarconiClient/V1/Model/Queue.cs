
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

namespace MarconiClient.V1.Model
{
    public class Queue
    {
        private int ExistCheck = 0;
        private bool Exists = false;
        public string Name { get; set; }
        public string Uri { get; set; }
        private QueueStats _Stats;
        //public bool Exists { get; set; }
        public QueueStats Stats { get { return _Stats; } }
        private Request request;



        public Queue(string name, string uri)
        {
            this.Name = name;
            this.Uri = uri;
            request = Request.getInstance();
            updateStats();
            //Exists = true;

        }

        public async void updateStats()
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.get(Uri + "/stats");
                string json = await response.Content.ReadAsStringAsync();
                _Stats = JsonConvert.DeserializeObject<QueueStats>(json);
            }
            else
            {
                throw new QueueMissingException();
            }
        }
        public async void setMetadata(object metadata)
        {
            if (await CheckExist())
            {
                string json = JsonConvert.SerializeObject(metadata);
                HttpResponseMessage response = await request.post(Uri + "/metadata", json);
                response.EnsureSuccessStatusCode();
            }
            else
            {
                throw new QueueMissingException();
            }
        }
        public async Task<string> getMetadata()
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.get(Uri + "/metadata");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new QueueMissingException();
            }
        }

        public async void postMessage(Message message)
        {
            if (await CheckExist())
            {
                List<Message> messages = new List<Message>();
                messages.Add(message);
                postMessage(messages);
            }
            else
                throw new QueueMissingException();
        }
        public async void postMessage(List<Message> messages)
        {
            if (await CheckExist())
            {
                string postbody = JsonConvert.SerializeObject(messages);
                HttpResponseMessage response = await request.post(Uri + "/messages", postbody);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine(json);
                JObject jsonObj = JObject.Parse(json);
                int counter = 0;
                foreach (var item in jsonObj["resources"])
                {
                    Message m = messages[counter];
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
        public async Task<Message> getMessage(string ID)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.get(Uri + "/messages/" + ID);
                response.EnsureSuccessStatusCode();
                JObject jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());
                return Message.Create(jsonObj);
            }
            else
                throw new QueueMissingException();

        }
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
        public async Task<Tuple<List<Message>, string>> getMessages(string marker, int limit, bool echo, bool include_claimed)
        {
            if (await CheckExist())
            {
                StringBuilder parameters = new StringBuilder("?");
                if (marker != null || marker != "")
                    parameters.Append("marker=" + marker + "&");
                parameters.Append("limit=" + limit + "&");
                parameters.Append("echo=" + echo + "&");
                parameters.Append("include_claimed=" + include_claimed);

                HttpResponseMessage response = await request.get(Uri + "/messages" + parameters.ToString());
                response.EnsureSuccessStatusCode();
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

        private async Task<bool> CheckExist()
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

        public async Task<bool> deleteMessage(string ID)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.delete(Uri + "/messages/" + ID);
                return response.IsSuccessStatusCode;
            }
            else
                throw new QueueMissingException();
        }
        public async Task<bool> deleteMessage(string ID, string claimID)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.delete(Uri + "/messages/" + ID + "?claim_id=" + claimID);
                return response.IsSuccessStatusCode;
            }
            else
                throw new QueueMissingException();
        }
        public async Task<bool> deleteMessages(string ID, params string[] values)
        {
            if (await CheckExist())
            {
                StringBuilder parameters = new StringBuilder("?ids=" + ID);
                foreach (string id in values)
                {
                    parameters.Append("," + id);
                }
                HttpResponseMessage response = await request.delete(Uri + "/messages" + parameters.ToString());
                return response.IsSuccessStatusCode;
            }
            else
                throw new QueueMissingException();
        }
        public string ToString()
        {
            return Name;
        }

        public async void delete()
        {
            await request.delete(Uri);
        }

        public async Task<List<Claim>> claim(int ttl, int grace)
        {
            return await claim(ttl, grace, 10);
        }

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
                string json = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine(json);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
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
        public async Task<bool> patchClaim(string claimid, int ttl)
        {
            if (await CheckExist())
            {
                Dictionary<string, int> claimBody = new Dictionary<string, int>();
                claimBody.Add("ttl", ttl);
                HttpResponseMessage response = await request.patch(Uri + "/claims/" + claimid, JsonConvert.SerializeObject(claimBody));
                return response.IsSuccessStatusCode;
            }
            else
                throw new QueueMissingException();
        }
        public async Task<bool> releaseClaim(string claimid)
        {
            if (await CheckExist())
            {
                HttpResponseMessage response = await request.delete(Uri + "/claims/" + claimid);
                return response.IsSuccessStatusCode;
            }
            else
                throw new QueueMissingException();

        }
  

    }   
    
}
