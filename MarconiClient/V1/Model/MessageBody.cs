using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarconiClient.V1.Model
{
    public class MessagePostBody
    {
        List<Message> messages;
        
        public MessagePostBody(Message message)
        {
            messages = new List<Message>();
            messages.Add(message);
        }
        public MessagePostBody(List<Message> messages)
        {
            this.messages = messages;
        }

    }

}
