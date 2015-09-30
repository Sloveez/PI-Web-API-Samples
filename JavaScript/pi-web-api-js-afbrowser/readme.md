#pi-web-api-js-browser
A JavaScript application for exploring the AF hierarchy of your PI System. 

##Getting Started
You can obtain the Visual Studio 2015 solution and the associated files from the [PI-Web-API-Samples/JavaScript/pi-web-api-js-afbrowser](./pi-web-api-js-afbrowser) folder. The code was tested with PI Web API 2015 R2. You will need a PI Web API server to connect to, as well as a built hierarchy within the AF Servers to look at.

In your own environment, change the PI Web API endpoint at https://myserver/piwebapi/assetservers by replacing myserver with the name of your PI Web API instance. In addition, the JavaScript aplication is using Basic Authentication to authentication to the PI Web API server, and you will need to provide the Base64-encoded string of your username:password in the loadChildren function. Don't forget to configure CORS settings for your PI Web API server accordingly. 

##Documentation
This example is adopted from the Getting Started section of the PI Web API help guide (https://myserver/piwebapi/help/getting-started). Changes include:
* Using Basic Authentication
* Added title and color
