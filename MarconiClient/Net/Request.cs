using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarconiClient
{
    class Request
    {
        public string ClientId { get; set; }
        private WebRequestHandler handler;
        private HttpClient client;
        private static Request instance;

        private Request()
        {
            handler = new WebRequestHandler();
            client = new HttpClient();
        }
        public static Request getInstance()
        {
            if (instance == null)
                instance = new Request();
            return instance;
        }

        public async Task<HttpResponseMessage> get(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Client-ID", ClientId);
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        public async Task<HttpResponseMessage> put(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Headers.Add("Client-ID", ClientId);
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        public async Task<HttpResponseMessage> post(string uri, string body)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Headers.Add("Client-ID", ClientId);
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
        public async Task<HttpResponseMessage> delete(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
            request.Headers.Add("Client-ID", ClientId);
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
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
