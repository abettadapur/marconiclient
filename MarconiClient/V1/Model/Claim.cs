using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MarconiClient.V1.Model
{
    /// <summary>
    /// Object representing a claim. Contains the claim ID and the message associated with that id
    /// </summary>
    public class Claim
    {
        private string _id;
        private List<Message> _messages;
        /// <summary>
        /// Gets the message associated with the claim.
        /// </summary>
        /// <value>
        /// The message associated with the claim.
        /// </value>
        public List<Message> Messages { get { return _messages; } }

        /// <summary>
        /// Returns the claim Id for the claim
        /// </summary>
        /// <value>
        /// The claim Id for the claim
        /// </value>
        public string ClaimID { get { return _id; } }

        /// <summary>
        /// Creates a new claim
        /// </summary>
        /// <param name="messages">The claimed messages.</param>
        /// <param name="id">The claim id.</param>
        internal Claim(List<Message> messages, string id)
        {
            _messages = messages;
            _id = id;
        }

        /// <summary>
        /// Creates a new claim from a json object
        /// </summary>
        /// <param name="jsonObj">The json object to create the claim from</param>
        /// <returns>The created claim object</returns>
        /*public static Claim Create(JObject jsonObj)
        {
            string strTargetString = @"" + jsonObj["href"].ToString();
            var parsedQuery = HttpUtility.ParseQueryString(strTargetString.Split('?')[1]);
            string claimid = parsedQuery["claim_id"];
            return new Claim(Message.Create(jsonObj), claimid);
        }*/

    }
}
