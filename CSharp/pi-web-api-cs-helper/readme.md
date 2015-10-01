# pi-web-api-cs-helper
A simple wrapper around the .NET HttpClient class that to allow easy access to PI Web API in C#. 

##Getting Started
You can obtain the Visual Studio 2015 solution and the associated files from the [PI-Web-API-Samples/CSharp/pi-web-api-cs-helper](./) folder. The code was tested with PI Web API 2015 R2. You will need a working instance of a PI Web API server and at lease one PI Data Archive to connect to.

##Projects

###pi-web-api-cs-helper
Detail implementation and description of the helper part of the project can be found in the [blog post] (https://pisquare.osisoft.com/community/developers-club/blog/2015/08/11/working-with-pi-web-api-httpclient-in-c) in PI Developers Club.

###pi-web-api-cs-readwritetag
In your own environment, change the PI Web API endpoint https://myserver/piwebapi by replacing myserver with the name of your PI Web API instance (in the Program.js file located in the pi-web-api-cs-readwritetag project).
