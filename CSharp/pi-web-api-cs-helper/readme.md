# pi-web-api-cs-helper
A simple wrapper around the .NET HttpClient class that allows easy access to PI Web API in C#. A separate C# console application (pi-web-api-cs-readwritetag) using the wrapper is also included.

##Getting Started
You can obtain the Visual Studio 2015 solution and the associated files from the [PI-Web-API-Samples/CSharp/pi-web-api-cs-helper](./) folder. The code was tested with PI Web API 2015 R2. You will need a working instance of a PI Web API server and at lease one PI Data Archive to connect to.

##Projects

###pi-web-api-cs-helper
This is a wrapper around HttpClient for PI Web API. Detail implementation and description of the helper part of the project can be found in the first part of the [blog post] (https://pisquare.osisoft.com/community/developers-club/blog/2015/08/11/working-with-pi-web-api-httpclient-in-c) in PI Developers Club.

To expand on the helper, feel free to add settings (e.g. [Timeout](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.timeout(v=vs.118).aspx)) and/or other headers (e.g. [CacheControl](https://msdn.microsoft.com/en-us/library/system.net.http.headers.httprequestheaders.cachecontrol(v=vs.118).aspx)) to the HttpClient appropriate for your application. In addition, this example only contains helper methods to submit GET and POST requests. Feel free to add helper methods to send other types of HTTP requests. 

###pi-web-api-cs-readwritetag
A C# console application that displays the list of PI Data Archive available for the PI Web API server instance, and allows the user to select a PI Data Archive and a specific tag. Users can then either read the current value of the tag or write a new value (at current time) to the tag. It uses basic authentication to communicate with PI Web API. The application will stop when users select to exit, or when the response messages from PI Web API do not indicate a success.

In your own environment, change the PI Web API endpoint at https://myserver/piwebapi by replacing myserver with the name of your PI Web API instance (in the Program.cs file located in the pi-web-api-cs-readwritetag project).
