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
using System.Threading.Tasks;
using System.Web.Mvc;
using pi_web_api_aspnet_search.Models;
using pi_web_api_cs_helper;

namespace pi_web_api_aspnet_search.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            SearchModel model = new SearchModel();
            List<SelectListItem> options = new List<SelectListItem>();
            options.Add(new SelectListItem { Text = "all", Value = "all", Selected = true });
            options.Add(new SelectListItem { Text = "name", Value = "name" });
            options.Add(new SelectListItem { Text = "description", Value = "description" });
            options.Add(new SelectListItem { Text = "afcategories", Value = "afcategories" });
            options.Add(new SelectListItem { Text = "afelementtemplate", Value = "afelementtemplate" });
            options.Add(new SelectListItem { Text = "attributename", Value = "attributename" });
            options.Add(new SelectListItem { Text = "attributedescription", Value = "attributedescription" });
            model.Options = options;

            return View(model);
        }

        [HttpPost, ActionName("Search")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(SearchModel model)
        {
            PIWebAPIClient client = new PIWebAPIClient("https://myserver/piwebapi");

            string query = model.Option.Equals("all") ? model.Query : model.Option + ":" + model.Query;
            int count = model.Count.HasValue ? (int)model.Count : 10;
            
            try
            {
                dynamic result = await client.SearchAsync(query, null, "name;itemtype;webid", count);
                List<SearchResult> searchResults = new List<SearchResult>();
                for (int i = 0; i < result.Items.Count; i++)
                {
                    SearchResult searchResult = new SearchResult();
                    searchResult.Name = result.Items[i].Name.Value;
                    searchResult.ItemType = result.Items[i].ItemType.Value;
                    searchResult.WebId = result.Items[i].WebID.Value;
                    searchResults.Add(searchResult);
                }
                return View(searchResults);
            }
            catch (Exception)
            {
                return View("Error");
                throw;
            }
            finally
            {
                client.Dispose();
            }
            
        }

    }
}