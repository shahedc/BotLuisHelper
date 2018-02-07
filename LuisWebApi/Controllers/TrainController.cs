﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace LuisWebApi.Controllers
{
    public class TrainController : ApiController
    {
        // TODO: move to config file (LUIS app info)
        static string appID = "<APP_ID>";
        static string appVersion = "0.1";
        static string key = "<APP_KEY>";
        static string host = "https://westus.api.cognitive.microsoft.com";
        static string path = "/luis/api/v2.0/apps/" + appID + "/versions/" + appVersion + "/";

        // GET api/train
        public async Task<string> Get()
        {
            var response = await SendGet(host + path + "train");
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        // GET api/train/5
        public string Get(int id)
        {
            return "train value";
        }

        // POST api/train
        public async Task<string> Post([FromBody]string utterText)
        {
            string uri = host + path + "train";
            var inputData = File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/utterances.json"));
            var response = await SendPost(uri, inputData);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        async static Task<HttpResponseMessage> SendGet(string uri)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                return await client.SendAsync(request);
            }
        }

        async static Task<HttpResponseMessage> SendPost(string uri, string requestBody)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "text/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                return await client.SendAsync(request);
            }
        }
        // PUT api/utterance/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/utterance/5
        public void Delete(int id)
        {
        }
    }
}
