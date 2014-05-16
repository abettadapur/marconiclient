using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MarconiClient.V1_1.Model
{
    /// <summary>
    /// Object representing a claim. Contains the claim ID and the message associated with that id
    /// </summary>
    public class Claim
    {
        private string _id;
        private Message _message;

        /// <summary>
        /// Gets the message associated with the claim.
        /// </summary>
        /// <value>
        /// The message associated with the claim.
        /// </value>
        public Message Message { get { return _message; } }

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
        /// <param name="message">The message.</param>
        /// <param name="id">The claim id.</param>
        internal Claim(Message message, string id)
        {
            _message = message;
            _id = id;
        }

        /// <summary>
        /// Creates a new claim from a json object
        /// </summary>
        /// <param name="jsonObj">The json object to create the claim from</param>
        /// <returns>The created claim object</returns>
        public static Claim Create(JObject jsonObj)
        {
            string claimid = jsonObj["claim"]["id"].ToString();
            return new Claim(Message.Create(jsonObj), claimid);
        }

    }
}
