//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarconiTest;

namespace NodeGrooverClient.Model
{
    public class Status
    {
       // [JsonProperty("state")]
        public string playState { get; set; }
       // [JsonProperty("length")]
        public int currentSongLength { get; set; }
       // [JsonProperty("time")]
        public int songTime{get;set;}
        public double position { get; set; }
        public int volume { get; set; }
        public Song[] queue { get; set; }
        //public UriInfo information { get; set; }

        public void updateStatus(Status status)
        {
            playState = status.playState;
            currentSongLength = status.currentSongLength;
            songTime = status.songTime;
            position = status.position;
            volume = status.volume;
        }

    }

}
