using System.Web;
using System.Web.Mvc;

namespace pi_web_api_aspnet_search
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
