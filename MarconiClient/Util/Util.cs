using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MarconiClient.Util
{
    class Util
    {
        public async static Task throwException(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                
                string respstr = await response.Content.ReadAsStringAsync();
                JObject errorObj = JObject.Parse(respstr);
                JToken desc = errorObj["description"];
                JToken title = errorObj["title"];
                string descstr = "";
                string titlestr = "";
                if (desc != null)
                    descstr = desc.ToString();
                if (title != null)
                    titlestr = title.ToString();
                throw new HttpException(400, title + ": " + descstr);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new HttpException(404, "The resource was not found. Has it been created?");
            else if (response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                throw new HttpException(405, "The called method does not exist on the object");
            else
            {
                throw new HttpException("Unknown error occurred");
            }


        }
    }
}
