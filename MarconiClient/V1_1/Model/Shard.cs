using MarconiClient.Net;
using MarconiClient.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarconiClient.V1_1.Model
{
    /// <summary>
    /// An object that represents a marconi database shard
    /// </summary>
    public class Shard
    {
        private string _name;
        private string _uri;
        private int _weight;
        private string _options;
        private string marconi_uri;
        private IRequest request;


        /// <summary>
        /// Gets the name of the shard
        /// </summary>
        /// <value>
        /// The name of the shard
        /// </value>
        public string Name { get { return _name; } }


        /// <summary>
        /// Gets the URI of the shard
        /// </summary>
        /// <value>
        /// The URI of the shard
        /// </value>
        [JsonProperty("uri")]
        public string Uri { get { return _uri; } }


        /// <summary>
        /// Gets the weight of the shard
        /// </summary>
        /// <value>
        /// The weight of the shard. Must be greater than 0
        /// </value>
        [JsonProperty("weight")]
        public int Weight { get { return _weight; } }


        /// <summary>
        /// Gets the options for the shard as a JSON string
        /// </summary>
        /// <value>
        /// The options for the shard
        /// </value>
        [JsonConverter(typeof(RawJsonConverter))]
        [JsonProperty("options")]
        public string Options { get { return _options; } }


        /// <summary>
        /// Initializes a new instance of the <see cref="Shard"/> class.
        /// </summary>
        /// <param name="name">The name of the shard</param>
        /// <param name="uri">The URI of the shard</param>
        /// <param name="weight">The weight of the shard</param>
        /// <param name="options">The options for the shard</param>
        /// <param name="marconi_uri">The marconi_uri to access the shard</param>
        /// <param name="request">A request object for HTTP requests</param>
        internal Shard(string name, string uri, int weight, object options, string marconi_uri, IRequest request)
        {
            _name = name;
            _uri = uri;
            _weight = weight;
            if(options!=null)
                _options = JsonConvert.SerializeObject(options);
            else
                _options = "";

            this.request = request;
            this.marconi_uri = marconi_uri;
        }

        /// <summary>
        /// Creates a shard from a json object
        /// </summary>
        /// <param name="jobj">The object to create from.</param>
        /// <param name="url">The marconi-url for the shard.</param>
        /// <param name="request">A request object for HTTP requests</param>
        /// <returns>The newly created shard</returns>
        public static Shard Create(JObject jobj, string url, IRequest request)
        {
            int weight;
            int.TryParse(jobj["weight"].ToString(), out weight);

            string uri = jobj["uri"].ToString();

            string href = @""+jobj["href"].ToString();
            string regexstr = @"^/[a-zA-Z0-9\.\-]*/shards/([a-zA-Z0-9_\-]*)";
            Regex regex = new Regex(regexstr, RegexOptions.None);
            string name = regex.Split(href)[1];

            string options = jobj["options"].ToString();

            Shard s = new Shard(name, uri, weight, null, url+"/shards/"+name, request);
            s._options = options;

            return s;
            
        }

        /// <summary>
        /// Changes the URI and issues a patch request.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public async void changeUri(string uri)
        {
            _uri = uri;
            Dictionary<string, string> body = new Dictionary<string, string>();
            body["uri"] = _uri;
            HttpResponseMessage response = await request.patch(marconi_uri, JsonConvert.SerializeObject(body));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Changes the weight and issues a patch request
        /// </summary>
        /// <param name="weight">The weight.</param>
        public async void changeWeight(int weight)
        {
            _weight = weight;
            Dictionary<string, int> body = new Dictionary<string, int>();
            body["weight"] = _weight;
            HttpResponseMessage response = await request.patch(marconi_uri, JsonConvert.SerializeObject(body));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Changes the options and issues a patch request
        /// </summary>
        /// <param name="options">The options.</param>
        public async void changeOptions(object options)
        {
            _options = JsonConvert.SerializeObject(options);
            HttpResponseMessage response = await request.patch(marconi_uri, JsonConvert.SerializeObject(options));
            response.EnsureSuccessStatusCode();

        }

        /// <summary>
        /// Deletes the shard.
        /// </summary>
        public async void deleteShard()
        {
            HttpResponseMessage response = await request.delete(marconi_uri);
            response.EnsureSuccessStatusCode();
        }



    }
}
