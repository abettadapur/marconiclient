using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarconiClient.V1_1.Model
{
    public class QueueStats
    {
        [JsonProperty("messages")]
        public MessageStats Messages { get; set; }   
    }

    public class MessageStats
    {
        [JsonProperty("free")]
        public int Free { get; set; }
        [JsonProperty("claimed")]
        public int Claimed { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }

    }
}
