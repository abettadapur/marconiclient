using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarconiClient.Net
{
    /// <summary>
    /// Default HTTP request handler for marconi requests.
    /// </summary>
    public class MarconiRequest : MarconiClient.Net.IRequest
    {
        private string _clientid;
        public string ClientId { get { return _clientid; } set { _clientid = value; } }
        private WebRequestHandler handler;
        private HttpClient client;
        private static MarconiRequest instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarconiRequest"/> class.
        /// </summary>
        public MarconiRequest()
        {
            handler = new WebRequestHandler();
            client = new HttpClient();
        }

        /// <summary>
        /// Performs a get request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// HttpResponseMessage
        /// </returns>
        public async Task<HttpResponseMessage> get(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Client-ID", ClientId);
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        /// <summary>
        /// Performs a put request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="body">The body.</param>
        /// <returns>
        /// HttpResponseMessage
        /// </returns>
        public async Task<HttpResponseMessage> put(string uri, string body)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Headers.Add("Client-ID", ClientId);
            if(body!=null&&body!="")
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        /// <summary>
        /// Performs a post request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="body">The body.</param>
        /// <returns>
        /// HttpResponseMessage
        /// </returns>
        public async Task<HttpResponseMessage> post(string uri, string body)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Headers.Add("Client-ID", ClientId);
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        /// <summary>
        /// Performs a delete request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// HttpResponseMessage
        /// </returns>
        public async Task<HttpResponseMessage> delete(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
            request.Headers.Add("Client-ID", ClientId);
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        /// <summary>
        /// Performs a patch request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="body">The body of the request</param>
        /// <returns>
        /// HttpResponseMessage
        /// </returns>
        public async Task<HttpResponseMessage> patch(string uri, string body)
        {
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Headers.Add("Client-ID", ClientId);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        
    }
}
