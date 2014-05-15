using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarconiClient.Util
{
    /// <summary>
    /// Exception that is thrown when a queue object cannot be found
    /// </summary>
    public class QueueMissingException : Exception
    {
        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get
            {
                return "The queue object has not been created or has been deleted";
            }
        }
    }
}
