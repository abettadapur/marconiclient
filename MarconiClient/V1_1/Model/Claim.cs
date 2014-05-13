using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MarconiClient.V1_1.Model
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
            string strTargetString = @"" + jsonObj["href"].ToString();
            var parsedQuery = HttpUtility.ParseQueryString(strTargetString.Split('?')[1]);
            string claimid = parsedQuery["claim_id"];
            return new Claim(Message.Create(jsonObj), claimid);
        }

    }
}
