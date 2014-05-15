
using MarconiClient.Net;
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
    /// <summary>
    /// The top level object for interfacing with Marconi. Creates queues.
    /// </summary>
    public class Client 
    {
        /// <summary>
        /// Gets or sets the hostname for the marconi instance
        /// </summary>
        /// <value>
        /// The hostname for the marconi instance
        /// </value>
        public string Host { get; set; }


        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }


        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        private string url;
        private IRequest request;


        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="version">The version.</param>
        /// <param name="request">The request object to make HTTP requests.</param>
        public Client(string host, int port, string version, IRequest request)
        {
            this.Host = host;
            this.Port = port;
            this.Version = version;
            this.url = Host + ":" + Port + "/" + Version;
            this.request = request;
            request.ClientId = "F93812A4-D9E4-11E3-BB74-A91C9A125B5A";
        }

        /// <summary>
        /// Creates a new Queue
        /// </summary>
        /// <param name="name">The name of the new queue</param>
        /// <returns>The new queue</returns>
        public async Task<Queue> createQueue(string name)
        {
                string uri =  url+"/queues/"+name;
                HttpResponseMessage response = await request.put(uri,"");
                if (!response.IsSuccessStatusCode)
                   await Util.Util.throwException(response); 
                Queue queue = new Queue(name, uri,request);
                return queue;
        }

        /// <summary>
        /// Gets the queues in the marconi instance.
        /// </summary>
        /// <returns>A list of queues</returns>
        public async Task<List<Queue>> getQueues()
        {
                List<Queue> queues = new List<Queue>();
                string uri = url + "/queues";
                HttpResponseMessage response = await request.get(uri);
                if (!response.IsSuccessStatusCode)
                    await Util.Util.throwException(response); 

                string json = await response.Content.ReadAsStringAsync();
                JObject jobj = JObject.Parse(json);
                foreach(var queue in jobj["queues"])
                {
                    Queue newQueue = new Queue(queue["name"].ToString(), Host+":"+Port + queue["href"].ToString(), request);
                    queues.Add(newQueue);
                }
               
                return queues;
        }


        /// <summary>
        /// Determines whether [is queue created] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Is the queue created?</returns>
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
