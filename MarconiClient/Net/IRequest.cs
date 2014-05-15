using System;
namespace MarconiClient.Net
{
    /// <summary>
    /// An object that makes HTTP requests
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Gets or sets the client identifier. 
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        string ClientId { get; set; }

        /// <summary>
        /// Performs a delete request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>HttpResponseMessage</returns>
        System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> delete(string uri);


        /// <summary>
        /// Performs a get request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>HttpResponseMessage</returns>
        System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> get(string uri);


        /// <summary>
        /// Performs a patch request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="body">The body of the request</param>
        /// <returns>HttpResponseMessage</returns>
        System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> patch(string uri, string body);


        /// <summary>
        /// Performs a post request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="body">The body.</param>
        /// <returns>HttpResponseMessage</returns>
        System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> post(string uri, string body);


        /// <summary>
        /// Performs a put request
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="body">The body.</param>
        /// <returns>HttpResponseMessage</returns>
        System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> put(string uri, string body);
    }
}
