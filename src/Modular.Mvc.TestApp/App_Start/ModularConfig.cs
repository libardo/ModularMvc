using Modular.Mvc.TestApp.Modules.Management.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modular.Mvc.TestApp
{
    public class ModularConfig
    {
        public static void RegisterModules(System.Web.Routing.RouteCollection routes, System.Web.Mvc.ViewEngineCollection viewEngines)
        {
            // The default means the calling assembly (this one) is scanned for controllers below the path /Modules
            Modular.Mvc.Initializer.Default
                // A custom route for EditUserController accepts userId as the last route segment
                .Route<EditUserController>("{userId}",
                    defaults: new { userId = 0 },
                    constraints: new { userId = "\\d" })
                .Configure(s => 
                    {
                        // advanced configuration here
                    })
                // Performs the actual registration
                .Register(routes, viewEngines);
        }
    }
}