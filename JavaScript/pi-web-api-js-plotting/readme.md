#pi-web-api-js-plotting
A JavaScript application for plotting PI tag values between a specified start and end time.

##Getting Started
You can obtain the Visual Studio 2015 solution and the associated files from the [PI-Web-API-Samples/JavaScript/pi-web-api-js-plotting](./) folder. The code was tested with PI Web API 2015 R2. You will need a working instance of a PI Web API server and a PI Data Archive to connect to.

In your own environment, make the following changes to the app.js file located in the Scripts folder.
* Change the PI Web API endpoint at https://myserver/piwebapi by replacing myserver with the name of your PI Web API instance.
* Change the piServer variable from "MyPIDataArchive" to the name of your PI Data Archive. 
* In addition, the JavaScript aplication is using Basic Authentication to authenticate to the PI Web API server, and you will need to provide the Base64-encoded string of your username:password in the getAjax function. 

Don't forget to configure CORS settings for your PI Web API server accordingly.

##Documentation
###How does the application work?
On page load, this single-page JavaScript application submits a GET Http request to the PI Web API server by using the GetByName action in the DataServer controller to get the WebId of the PI Data Archive.

User can then type in a tag mask (e.g. sin*), click on the "Search for tags" button, and obtain the list of tags in a dropdown box. The application achieves this by using the GetPoints action in the DataServer controller.

User can then select a tag, enter the start and end time, and click on the "plot" button to display a time-series graph. The format of the time string is specified in the PI Web API help file under ["Time Strings"](https://techsupport.osisoft.com/Documentation/PI-Web-API/help/topics/time-strings.html). If no start and end time is entered, the default is *-1d (1 day ago) till * (now). The application is using the GetPlot action in the Stream controller. The number of intervals to plot over is hard-coded to match the number of pixels for the width of the plot. 

###Plotting Library
The JavaScript plotting library [flot](http://www.flotcharts.org/) was used in this project. Since flot accepts time series data in terms of [timestamp value] pair, where timestamp is a Javascript timestamp (the number of milliseconds since January 1, 1970 00:00:00 UTC), we are converting the Json time string from PI Web API with
```
new Date(jsonTimeString).getTime()
```
Refer to the time series data section in the [flot API reference](https://github.com/flot/flot/blob/master/API.md#time-series-data) for more information.
