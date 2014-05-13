
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

namespace MarconiClient.V1_1.Model
{
    public class Queue
    {

        public string Name { get; set; }
        public string Uri { get; set; }
        private QueueStats _Stats;
        public bool Exists { get; set; }
        public QueueStats Stats { get { return _Stats; } }
        private Request request;



        public Queue(string name, string uri)
        {
            this.Name = name;
            this.Uri = uri;
            request = Request.getInstance();
            updateStats();
            Exists = true;

        }

        public async void updateStats()
        {
            if (Exists)
            {
                HttpResponseMessage response = await request.get(Uri + "/stats");
                string json = await response.Content.ReadAsStringAsync();
                _Stats = JsonConvert.DeserializeObject<QueueStats>(json);
            }
        }
        
        [Obsolete]
        public async Task<string> getMetadata()
        {
            HttpResponseMessage response = await request.get(Uri + "/metadata");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async void postMessage(Message message)
        {
            List<Message> messages = new List<Message>();
            messages.Add(message);
            postMessage(messages);
        }
        public async void postMessage(List<Message> messages)
        {
            if (Exists)
            {
                string postbody = JsonConvert.SerializeObject(messages);
                HttpResponseMessage response = await request.post(Uri + "/messages", postbody);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine(json);
                JObject jsonObj = JObject.Parse(json);
                int counter = 0;
                foreach(var item in jsonObj["links"])
                {
                    Message m = messages[counter];
                    string strRegex = @"^messages/([a-zA-Z0-9]*)";
                    Regex myRegex = new Regex(strRegex, RegexOptions.None);
                    string strTargetString = @""+item["href"].ToString();
                    m.ID = myRegex.Split(strTargetString)[1];
                }
                
                
            }
            
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
            HttpResponseMessage response = await request.get(Uri + "/messages/" + ID);
            response.EnsureSuccessStatusCode();
            JObject jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());
            return Message.Create(jsonObj);

        }
        public async Task<List<Message>> getMessages(string ID, params string[] values)
        {
            StringBuilder parameters = new StringBuilder("?ids=" + ID);
            foreach(string id in values)
            {
                parameters.Append("," + id);
            }
            HttpResponseMessage response = await request.get(Uri + "/messages" + parameters.ToString());
            response.EnsureSuccessStatusCode();
            List<Message> messages = new List<Message>();
            
            JArray jsonArr = JArray.Parse(await response.Content.ReadAsStringAsync());
            foreach(JObject message in jsonArr )
            {
                messages.Add(Message.Create(message));
            }
            return messages;
        }
        public async Task<Tuple<List<Message>, string>> getMessages(string marker, int limit, bool echo, bool include_claimed)
        {
            StringBuilder parameters = new StringBuilder("?");
            if (marker != null||marker!="")
                parameters.Append("marker=" + marker+"&");
            parameters.Append("limit=" + limit+"&");
            parameters.Append("echo=" + echo+"&");
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

        public async Task<bool> deleteMessage(string ID)
        {
            HttpResponseMessage response = await request.delete(Uri + "/messages/" + ID);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> deleteMessage(string ID, string claimID)
        {
            HttpResponseMessage response = await request.delete(Uri + "/messages/" + ID + "?claim_id=" + claimID);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> deleteMessages(string ID, params string[] values)
        {
            StringBuilder parameters = new StringBuilder("?ids=" + ID);
            foreach (string id in values)
            {
                parameters.Append("," + id);
            }
            HttpResponseMessage response = await request.delete(Uri + "/messages" + parameters.ToString());
            return response.IsSuccessStatusCode;
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
            if (limit <= 0)
                return null;
            Dictionary<string, int> claimPostBody = new Dictionary<string, int>();
            claimPostBody["ttl"] = ttl;
            claimPostBody["grace"] = grace;
            string jsonbody = JsonConvert.SerializeObject(claimPostBody);
            HttpResponseMessage response = await request.post(Uri + "/claims?limit=" + limit,jsonbody);
            string json = await response.Content.ReadAsStringAsync();
            Console.Out.WriteLine(json);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                JObject jsonArr = JObject.Parse(json);
                List<Claim> claims = new List<Claim>();
                foreach (JObject jsonObj in jsonArr["messages"])
                {
                    claims.Add(Claim.Create(jsonObj));
                }
                return claims;
            }
            return null;
        }
        public async Task<Claim> getClaim(string claimid)
        {
            return null;  ///needs some thought
        }
        public async Task<bool> patchClaim(string claimid, int ttl)
        {
            Dictionary<string, int> claimBody = new Dictionary<string, int>();
            claimBody.Add("ttl", ttl);
            HttpResponseMessage response = await request.patch(Uri + "/claims/" + claimid, JsonConvert.SerializeObject(claimBody));
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> releaseClaim(string claimid)
        {
            HttpResponseMessage response = await request.delete(Uri + "/claims/" + claimid);
            return response.IsSuccessStatusCode;

        }
        public async Task<List<Message>> popMessages(int count)
        {
            HttpResponseMessage response = await request.delete(Uri + "/messages?pop=" + count);
            response.EnsureSuccessStatusCode();
            JObject jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());
            List<Message> messages = new List<Message>();
            foreach(JObject message in jsonObj["messages"])
            {
                messages.Add(Message.Create(message));
            }
            return messages;

        }
  

    }   
    
}
