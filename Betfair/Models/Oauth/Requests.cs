using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaf.xNet;

namespace Betfair
{
    class Requests
    {
        private HttpRequest HttpRequests()
        {
            HttpRequest request = new HttpRequest();
            request.UserAgentRandomize();
            request.KeepAlive = true;
            request.Reconnect = true;
            request.ReconnectLimit = 3;
            return request;
        }

        public async Task<HttpResponse> getRequest(string Url)
        {
            HttpRequest request = new HttpRequest();
            await Task.Run(() => { request= HttpRequests(); });
            var response = request.Get(Url);
            return response;
            
        }

        public async Task<HttpResponse> postRequest(string Url, RequestParams param)
        {
            HttpRequest request = new HttpRequest();
            await Task.Run(() => { request = HttpRequests(); });
            var response = request.Post(Url, param);
            return response;
        }
    }
}
