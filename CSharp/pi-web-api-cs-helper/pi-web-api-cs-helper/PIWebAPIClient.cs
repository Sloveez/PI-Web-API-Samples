using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace pi_web_api_cs_helper
{
    public class PIWebAPIClient
    {
        private HttpClient client;

        /// <summary>
        /// Initiating HttpClient using the default credentials.
        /// This can be used with Kerberos authentication for PI Web API.
        /// </summary>
        public PIWebAPIClient()
        {
            client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
        }

        /// <summary>
        /// Initializing HttpClient by providing a username and password. The basic authentication header is added to the HttpClient.
        /// This can be used with Basic authentication for PI Web API.
        /// </summary>
        /// <param name="userName">User name for basic authentication</param>
        /// <param name="password">Password for basic authentication</param>
        public PIWebAPIClient(string userName, string password)
        {
            client = new HttpClient();
            string authInfo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", userName, password)));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);
        }

        /// <summary>
        /// Async GET request. This method makes a HTTP GET request to the uri provided 
        /// and throws an exception if the response does not indicate a success.
        /// </summary>
        /// <param name="uri">Endpoint for the GET request.</param>
        /// <returns>Response from the server in a Json Object.</returns>
        public async Task<JObject> GetAsync(string uri)
        {
            HttpResponseMessage response = await client.GetAsync(uri);

            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
            return JObject.Parse(content);
        }

        /// <summary>
        /// Async POST request. This method makes a HTTP POST request to the uri provided
        /// and throws an exception if the response does not indicate a success.
        /// </summary>
        /// <param name="uri">Endpoint for the POST request.</param>
        /// <param name="data">Content for the POST request.</param>
        public async Task PostAsync(string uri, string data)
        {
            HttpResponseMessage response = await client.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));

            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
        }

        /// <summary>
        /// Calling the GetAsync method and waiting for the results.
        /// </summary>
        /// <param name="url">Endpoint for the GET request.</param>
        /// <returns>Response from the server in a Json Object.</returns>
        public JObject GetRequest(string url)
        {
            Task<JObject> t = this.GetAsync(url);
            t.Wait();
            return t.Result;
        }

        /// <summary>
        /// Calling the PostAsync method and waiting for the results.
        /// </summary>
        /// <param name="url">Endpoint for the POST request.</param>
        /// <param name="data">Content for the POST request.</param>
        public void PostRequest(string url, string data)
        {
            Task t = this.PostAsync(url, data);
            t.Wait();
        }

        /// <summary>
        /// Disposing the HttpClient.
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }
    }
}
