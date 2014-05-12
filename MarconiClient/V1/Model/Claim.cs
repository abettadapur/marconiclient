using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MarconiClient.V1.Model
{
    public class Claim
    {
        public Message Message { get; set; }
        public string ClaimID { get; set; }

        public Claim(Message message, string id)
        {
            Message = message;
            ClaimID = id;
        }
        public static Claim Create(JObject jsonObj)
        {
            string strRegex = @"^/[a-z0-9]*/queues/[a-zA-Z0-9\-]*/messages/([a-zA-Z0-9]*)(\?claim_id=([a-zA-Z0-9\-]*))?";
            Regex myRegex = new Regex(strRegex, RegexOptions.None);
            string strTargetString = @"" + jsonObj["href"].ToString();
            string[] splits = myRegex.Split(strTargetString);
            string claimid = splits[3];

            return new Claim(Message.Create(jsonObj), claimid);
        }

    }
}
