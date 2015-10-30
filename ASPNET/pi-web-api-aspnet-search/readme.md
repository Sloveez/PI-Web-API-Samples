# pi-web-api-aspnet-search
This ASP.NET MVC application allows user to enter a search query and perform a search for a selected category using the PI Web API Indexed Search.

##Getting Started
You can obtain the Visual Studio 2015 solution and the associated files from the [PI-Web-API-Samples/CSharp/pi-web-api-aspnet-search](./) folder. The code was tested with PI Web API 2015 R2 and 2015 R3. You will need a working instance of the PI Web API server, with the [PI Indexed Search feature](https://livelibrary.osisoft.com/LiveLibrary/content/en/web-api-v2/GUID-78EA0D36-5BDD-4F40-99FB-6D48D62B15DA) installed. You will need to set up the search crawler to index the PI Data Archive and/or the AF Database. For more information on how to set up the search, please refer to our PI Live Library documentation [here](https://livelibrary.osisoft.com/LiveLibrary/content/en/web-api-v2/GUID-D847876F-A9A3-4089-89C8-5205FBCF9198#addHistory=true&filename=GUID-16B3943B-0F1C-4BAF-94B0-0872AB00F797.xml&docid=GUID-16B3943B-0F1C-4BAF-94B0-0872AB00F797&inner_id=&tid=&query=&scope=&resource=&toc=false&eventType=lcContent.loadDocGUID-16B3943B-0F1C-4BAF-94B0-0872AB00F797). Note that you need to use Kerberos authentication in order to generate new indices (as of PI Web API 2015 R3).

In your own environment, change the PI Web API endpoint at https://myserver/piwebapi by replacing myserver with the name of your PI Web API instance (in HomeController.cs in the Controllers folder).

##How does the Application Work?
On the main page of this application, users can enter a search string, select a search category, and the maximum number of results returned. The application makes a HTTP GET request to the PI Web API server at the search/query endpoint ([Search controller > Query action](https://techsupport.osisoft.com/Documentation/PI-Web-API/help/controllers/search/actions/query.html)), the server then return the name, item type (e.g. PI Point, AF Element), as well as the WebId to the client. The results are displayed in a table on a separate page. 

##Implementation Details
Communication to the PI Web API is made via the .NET HttpClient. A reference has been added to the [pi-web-api-cs-helper](../../CSharp/pi-web-api-cs-helper) project to use the SearchAsync method. Please refer to the project [readme](../../CSharp/pi-web-api-cs-helper/readme.md) file for detail implementation and description of the helper.

In the Home controller, we populate the dropdown list to show the search categories that users can select from. 

When users post to the Search action in the Home controller, the PI Web API url endpoint was constructed based on the user-entered information:
```
string query = model.Option.Equals("all") ? model.Query : model.Option + ":" + model.Query;
int count = model.Count.HasValue ? (int)model.Count : 10;
```
We then call the SearchAsync method in the PIWebAPIClient wrapper to submit a GET request to the url:
```
dynamic result = await client.SearchAsync(query, null, "name;itemtype;webid", count);
```

Note that we are using Kerberos authentication to connect to PI Web API. As of PI Web API 2015 R3, the PI Web API Indexed Search will only crawl when Kerberos authentication is configured. You can, however, search for crawled indices using basic authentication.

From the HTTP response, we loop through the items collection and add each result to the SearchResult model:
```
List<SearchResult> searchResults = new List<SearchResult>();
for (int i = 0; i < result.Items.Count; i++)
{
    SearchResult searchResult = new SearchResult();
    searchResult.Name = result.Items[i].Name.Value;
    searchResult.ItemType = result.Items[i].ItemType.Value;
    searchResult.WebId = result.Items[i].WebID.Value;
    searchResults.Add(searchResult);
}
```

The SearchResult is then passed to the Search view to render the results on the page.
