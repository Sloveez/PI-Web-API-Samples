/***************************************************************************
   Copyright 2015 OSIsoft, LLC.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pi_web_api_cs_helper
{
    public class PIWebAPIClient
    {
        private HttpClient _client;
        private string _baseUrl;

        /// <summary>
        /// Initiating HttpClient using the default credentials.
        /// This can be used with Kerberos authentication for PI Web API.
        /// </summary>
        public PIWebAPIClient(string url)
        {
            _client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            _baseUrl = url;
        }

        /// <summary>
        /// Initializing HttpClient by providing a username and password. The basic authentication header is added to the HttpClient.
        /// This can be used with Basic authentication for PI Web API.
        /// </summary>
        /// <param name="userName">User name for basic authentication</param>
        /// <param name="password">Password for basic authentication</param>
        public PIWebAPIClient(string url, string userName, string password)
        {
            _client = new HttpClient();
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password)));
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);
            _baseUrl = url;
        }

        /// <summary>
        /// Async GET request. This method makes a HTTP GET request to the uri provided 
        /// and throws an exception if the response does not indicate a success.
        /// </summary>
        /// <param name="uri">Endpoint for the GET request.</param>
        /// <returns>Response from the server in a Json Object.</returns>
        public async Task<dynamic> GetAsync(string uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            
            dynamic content = await response.Content.ReadAsAsync<JToken>();
            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
            return content;
        }

        /// <summary>
        /// Async POST request. This method makes a HTTP POST request to the uri provided
        /// and throws an exception if the response does not indicate a success.
        /// </summary>
        /// <param name="uri">Endpoint for the POST request.</param>
        /// <param name="data">Content for the POST request.</param>
        public async Task PostAsync(string uri, string data)
        {
            HttpResponseMessage response = await _client.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
        }

        /// <summary>
        /// Get the list of PI Data Archives in "name, webId" pairs.
        /// </summary>
        /// <returns>Dictionary of PI Data Archives.</returns>
        public async Task<Dictionary<string, string>> GetDataServersWebIdAsync()
        {
            string url = _baseUrl + "/dataservers";
            dynamic result = await GetAsync(url);
            Dictionary<string, string> servers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in result.Items)
            {
                servers.Add(item.Name.Value, item.WebId.Value);
            }
            return servers;
        }

        /// <summary>
        /// Get the value of a particular PI tag.
        /// </summary>
        /// <param name="serverWebId">WebId of the PI Data Archive.</param>
        /// <param name="tagName">Name of the PI tag.</param>
        /// <returns>Value of the tag.</returns>
        public async Task<dynamic> GetTagValueAsync(string serverWebId, string tagName)
        {
            string url = await GetTagValueLink(serverWebId, tagName);
            dynamic result = await GetAsync(url);
            return result.Value;
        }

        /// <summary>
        /// Write a value at current time to a particular PI tag.
        /// </summary>
        /// <param name="serverWebId">WebId of the PI Data Archive.</param>
        /// <param name="tagName">Name of the PI tag.</param>
        /// <param name="valueToWrite">Value to write.</param>
        public async Task WriteTagValueAsync(string serverWebId, string tagName, string valueToWrite)
        {
            string url = await GetTagValueLink(serverWebId, tagName);
            object payload = new
            {
                Value = valueToWrite
            };
            string data = JsonConvert.SerializeObject(payload);
            await PostAsync(url, data);
        }

        /// <summary>
        /// Get the value link for a specific PI tag.
        /// </summary>
        /// <param name="serverWebId">WebId of the PI Data Archive.</param>
        /// <param name="tagName">Name of the PI tag.</param>
        /// <returns>Link to read/write a single value.</returns>
        private async Task<string> GetTagValueLink(string serverWebId, string tagName)
        {
            string url = _baseUrl + "/dataservers/" + serverWebId + "/points?nameFilter=" + HttpUtility.UrlEncode(tagName);
            dynamic result = await GetAsync(url);
            if (result.Items.Count > 0)
            {
                return result.Items[0].Links.Value;
            }
            else
            {
                throw new InvalidOperationException("Tag is not found.");
            }
        }

        public async Task<dynamic> SearchAsync(string query, string scope, string fields, int count = 10, int start = 0)
        {
            string url = _baseUrl + "/search/query?q=" + HttpUtility.UrlEncode(query) + "&count=" + count + "&start" + start;
            if (!string.IsNullOrEmpty(scope))
            {
                url += "&scope=" + scope;
            }
            if (!string.IsNullOrEmpty(fields))
            {
                url += "&fields=" + fields;
            }
            return await GetAsync(url);
        }
        
        /// <summary>
        /// Disposing the HttpClient.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
