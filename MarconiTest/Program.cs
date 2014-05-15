using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using MarconiClient.V1;
using MarconiClient.V1.Model;
using MarconiClient.Net;
using NodeGrooverClient.Model;
using System.Web;

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

            await Test1();
            await Test2();
            Console.ReadLine();
     
        }

        private static async Task Test1()
        {
            try
            {
                Client client = new Client("http://10.5.73.249", 8888, "v1", new MarconiRequest());
                Queue test1 = await client.createQueue("test1");
                Console.Out.WriteLine("Created " + test1.Name + " at " + test1.Uri);
                Console.Out.Write("Does it exist....");
                Console.Out.WriteLine(await client.isQueueCreated(test1.Name));
                Queue test2 = await client.createQueue("test2");
                Console.Out.WriteLine("Created " + test2.Name + " at " + test2.Uri);
                Console.Out.Write("Does it exist....");
                Console.Out.WriteLine(await client.isQueueCreated(test1.Name));

                List<Message> songMessages = new List<Message>();
                foreach (Song s in songs)
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
                    foreach (Message m in results)
                    {
                        Console.Out.WriteLine(m);
                    }

                } while (results.Count != 0);

                List<Claim> claims = await test1.claim(300, 300);
                foreach (Claim c in claims)
                {
                    Console.Out.Write("Claimed " + c.Message.ID + " with claim id" + c.ClaimID + "....");
                    await test1.deleteMessage(c.Message.ID, c.ClaimID);
                    Console.Out.WriteLine("");
                }

                Console.Out.Write("Deleting test1....");
                test1.delete();
                Console.Out.WriteLine(!await client.isQueueCreated(test1.Name));
                Console.Out.Write("Deleting test2....");
                test2.delete();
                Console.Out.WriteLine(!await client.isQueueCreated(test2.Name));
            }
            catch(Exception exception)
            {
                Console.Out.Write(exception.Message);
            }

            
        }
        public static async Task Test2()
        {
            try
            {
                MarconiClient.V1_1.Client client = new MarconiClient.V1_1.Client("http://10.5.73.249", 8888, "v1.1", new MarconiRequest());
                MarconiClient.V1_1.Model.Shard shard = await client.createShard("newShard", "http://127.0.0.1:27017", 100, null);
            }
            catch(HttpException ex)
            {
                Console.Out.Write(ex.Message);
            }
           // shard.deleteShard();
        }
    }
}
