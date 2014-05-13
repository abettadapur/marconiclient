using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using MarconiClient.V1;
using MarconiClient.V1.Model;
using NodeGrooverClient.Model;

namespace MarconiTest
{
    class Program
    {
        static List<Song> songs = new List<Song>()
        {
            new Song(){Name="Shake me Down", ArtistName="Cage the Elephant", ArtistID=1234567},
            new Song(){Name="Long Time", ArtistName="Boston", ArtistID=7654321},
            new Song(){Name="Nightswimming", ArtistName="REM", ArtistID=4567321},
            new Song(){Name="Landslide", ArtistName="Fleetwood Mac", ArtistID=3521764},
            new Song(){Name="Power", ArtistName="Kanye West", ArtistID=0909882},
            new Song(){Name="1901", ArtistName="Phoenix", ArtistID=2934820},
            new Song(){Name="All Star", ArtistName="Smashmouth", ArtistID=2398402},

        };
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }
        static async void MainAsync(string[] args)
        {
            Client client = new Client("http://10.5.73.220", 8888, "v1");
            Queue test1 = await client.createQueue("test1");
            Console.Out.WriteLine("Created " + test1.Name + " at " + test1.Uri);
            Queue test2 = await client.createQueue("test2");
            Console.Out.WriteLine("Created " + test2.Name + " at " + test2.Uri);

            List<Message> songMessages = new List<Message>();
            foreach(Song s in songs)
            {
                songMessages.Add(new Message(s));
            }
            test1.postMessage(songMessages);

            List<Message> results = new List<Message>();
            string marker = "";
            do
            {
                Tuple<List<Message>, string> result = await test1.getMessages(marker, 1, true, false);
                results = result.Item1;
                marker = result.Item2;
                foreach(Message m in results)
                {
                    Console.Out.WriteLine(m);
                }

            } while (results.Count != 0);

            List<Claim> claims = await test1.claim(300, 300);
            foreach(Claim c in claims)
            {
                Console.Out.Write("Claimed " + c.Message.ID + " with claim id" + c.ClaimID+"....");
                bool success = await test1.deleteMessage(c.Message.ID, c.ClaimID);
                Console.Out.WriteLine(success);
            }
            Console.ReadLine();
     
        }
    }
}
