//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGrooverClient.Model
{
    public class Song
    {
        public string Url { get; set; }
        public long SongID { get; set; }
        public string Name { get; set; }
        public long ArtistID { get; set; }
        public string ArtistName { get; set; }
        public long AlbumID { get; set; }
        public string AlbumName { get; set; }
        public int Count { get; set; }
        public bool Current { get; set; }
        public int Time { get; set; }
        public int Max { get; set; }

    }
}
