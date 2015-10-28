#pi-web-api-cs-readwritetag
A simple C# console application that displays the list of PI Data Archive available for the PI Web API server instance, and allows the user to select a PI Data Archive and a specific tag.

##Getting Started
You can obtain the Visual Studio 2015 solution and the associated files from the [PI-Web-API-Samples/CSharp/pi-web-api-cs-readwritetag](./) folder. The code was tested with PI Web API 2015 R2 and 2015 R3. You will need a working instance of a PI Web API server and at lease one PI Data Archive to connect to. In your own environment, change the PI Web API endpoint at https://myserver/piwebapi by replacing myserver with the name of your PI Web API instance (in the Program.cs file).

##Implementation Details
The console application displays the list of PI Data Archive available for the PI Web API server instance, and allows the user to select a PI Data Archive and a specific tag. Users can then either read the current value of the tag or write a new value (at current time) to the tag. It uses basic authentication to communicate with PI Web API. The application will stop when users select to exit, or when the response messages from PI Web API do not indicate a success.

Communication to the PI Web API is made via the .NET HttpClient. A reference has been added to the [pi-web-api-cs-helper](../pi-web-api-cs-helper) project to use the GetDataServersWebIdAsync, GetTagValueAsync, and WriteTagValueAsync methods.




