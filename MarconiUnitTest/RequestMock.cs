using MarconiClient.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarconiUnitTest
{
    class RequestMock : IRequest
    {
        public string _clientid;
        public string ClientId
        {
            get
            {
                return _clientid;
            }
            set
            {
                _clientid = value;
            }
        }

        public Task<System.Net.Http.HttpResponseMessage> delete(string uri)
        {
            throw new NotImplementedException();
        }

        public Task<System.Net.Http.HttpResponseMessage> get(string uri)
        {
            throw new NotImplementedException();
        }

        public Task<System.Net.Http.HttpResponseMessage> patch(string uri, string body)
        {
            throw new NotImplementedException();
        }

        public Task<System.Net.Http.HttpResponseMessage> post(string uri, string body)
        {
            throw new NotImplementedException();
        }

        public Task<System.Net.Http.HttpResponseMessage> put(string uri, string body)
        {
            throw new NotImplementedException();
        }
    }
}
