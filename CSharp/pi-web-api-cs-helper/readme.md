# pi-web-api-cs-helper
A simple wrapper around the .NET HttpClient class that allows easy access to PI Web API in C#. 

##Getting Started
You can obtain the Visual Studio 2015 solution and the associated files from the [PI-Web-API-Samples/CSharp/pi-web-api-cs-helper](./) folder. The code was tested with PI Web API 2015 R2 and 2015 R3. You will need a working instance of a PI Web API server and at lease one PI Data Archive to connect to.

##Implementation Details
This is a wrapper around HttpClient for using PI Web API in C#.

###Creating and Disposing the HttpClient
We are using the HttpClient to submit HTTP requests to PI Web API. The following is the constructors and methods to initialize a HttpClient intended to be used to make multiple HTTP requests.
```
private HttpClient _client;
private string _baseUrl;

public PIWebAPIClient(string url)
{
    _client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
    _baseUrl = url;
}

public PIWebAPIClient(string url, string userName, string password)
{
    _client = new HttpClient();
    string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password)));
    _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);
    _baseUrl = url;
}

public void Dispose()
{
    _client.Dispose();
}
```

To expand on the helper, feel free to add settings (e.g. [Timeout](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.timeout(v=vs.118).aspx)) and/or other headers (e.g. [CacheControl](https://msdn.microsoft.com/en-us/library/system.net.http.headers.httprequestheaders.cachecontrol(v=vs.118).aspx)) to the HttpClient appropriate for your application. In addition, although HttpClient does implement the IDisposable interface, many MSDN examples did not explicitly call Dispose(). We will include it for completion sake.
 
###Asynchronous GET and POST request
PI Web API represents all the resources in JSON objects, and we will return a dynamic object from our async GET method. For POST, we will be accepting a string payload in JSON format. If the response message indicates that the request is not successful, a HttpRequestException will be thrown with the response message.

```
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
``` 

At times, you might need to submit other HTTP requests to PI Web API (e.g. PUT, PATCH). While you can use HttpClient.PutAsync for a PUT request, it doesn’t have a method to support PATCH request out-of-the-box. If you are in a situation to make a PATCH call, there are many online examples to do so. For more information, refer to this previous discussion.
 
There are a few helper methods built to carry out various functions in PI Web API. E.g. this is an example of doing a indexed search in PI Web API (Search controller > Query action):
```
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
```

Feel free to contribute and add more functions!
