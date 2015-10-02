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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace pi_web_api_aspnet_search.Models
{
    public class SearchModel
    {
        [Required]
        [DisplayName("Query string")]
        public string Query { get; set; }

        [Required]
        [DisplayName("Query field")]
        public string Option { get; set; }

        public List<SelectListItem> Options { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a positive integer.")]
        public int? Count { get; set; }
    }
}