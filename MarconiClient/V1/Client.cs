
using MarconiClient.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarconiClient.V1
{
    public class Client 
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Version { get; set; }

        private string url;
        private Request request;

        public Client(string host, int port, string version)
        {
            this.Host = host;
            this.Port = port;
            this.Version = version;
            this.url = Host + ":" + Port + "/" + Version;
            request = Request.getInstance();
            request.ClientId = "F93812A4-D9E4-11E3-BB74-A91C9A125B5A";
        }

        public async Task<Queue> createQueue(string name)
        {
            try
            {
                string uri =  url+"/queues/"+name;
                HttpResponseMessage response = await request.put(uri);
                response.EnsureSuccessStatusCode();
                Queue queue = new Queue(name, uri);
                return queue;
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Queue>> getQueues()
        {
            try
            {
                List<Queue> queues = new List<Queue>();
                string uri = url + "/queues";
                HttpResponseMessage response = await request.get(uri);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject jobj = JObject.Parse(json);
                foreach(var queue in jobj["queues"])
                {
                    Queue newQueue = new Queue(queue["name"].ToString(), Host+":"+Port + queue["href"].ToString());
                    queues.Add(newQueue);
                }
               
                return queues;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> isQueueCreated(string name)
        {
            HttpResponseMessage response = await request.get(url + "/queues/" + name);
            return response.IsSuccessStatusCode;
        }


        
        /*public async Task<bool> deleteQueue(string name)
        {
            HttpResponseMessage response = await request.delete(url + "/queues/" + name);
            response.EnsureSuccessStatusCode();
            return true;
        }
        public async Task<bool> deleteQueue(Queue queue)
        {
            if (queue != null)
            {
                HttpResponseMessage response = await request.delete(url + "/queues/" + queue.Name);
                response.EnsureSuccessStatusCode();
            }
            return true;
        }*/


    }
}
