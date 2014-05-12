using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using MarconiClient.V1;
using MarconiClient.V1.Model;

namespace MarconiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }
        static async void MainAsync(string[] args)
        {
           
            //if (args[0] == "v1")
            Client client = new MarconiClient.V1.Client("http://10.5.73.12", 8888, "v1");
            //else
              //  client = new MarconiClient.V1_1.Client("http://10.5.73.130", 8888, "v1.1");
            Queue queue2 = await client.createQueue("newQueue2");
            Queue queue3 = await client.createQueue("newQueue3");
            Queue queue4 = await client.createQueue("newQueue4");
            Queue queue5 = await client.createQueue("newQueue5");
            Message m1 = new Message(queue4);
            Message m2 = new Message("Hello Marconi");
            queue3.postMessage(m2);
            queue3.postMessage(m1);
            queue4.delete();
            List<Claim> claims = await queue3.claim(800,800,10);
            //List<Message> messages = await queue3.getAllMessages();
            //List<Message> messages2 = await queue3.getMessages(m1.ID,m2.ID);
            //queue3.postMessage(messages[1]);
            List<Queue> queues = await client.getQueues();
            Console.Out.WriteLine(queues);
            Console.ReadLine();
        }
    }
}
