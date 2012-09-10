using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Modular.Mvc.Implementation
{
    public class ModularRoute : RouteBase
    {
        private Route route;
        private string controllerName;

        public ModularRoute(Route route, string controllerName)
        {
            this.route = route;
            this.controllerName = controllerName;
        }

        public string Url
        {
            get { return route.Url; }
        }

        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext)
        {
            return route.GetRouteData(httpContext);
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            if (controllerName.Equals(values["Controller"]))
                return route.GetVirtualPath(requestContext, values);
            return null;
        }
    }
}
