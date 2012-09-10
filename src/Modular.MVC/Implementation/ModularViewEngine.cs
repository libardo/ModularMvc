using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Modular.Mvc.Implementation
{
    public class ModularViewEngine : IViewEngine 
    {
        private VirtualPathProviderViewEngine viewEngine;
        private Type controllerType;

        public ModularViewEngine(VirtualPathProviderViewEngine viewEngine, Type controllerType, string folderPath, IEnumerable<string> fallbackPaths, IEnumerable<string> extensions)
        {
            this.viewEngine = viewEngine;
            this.controllerType = controllerType;
            viewEngine.AreaMasterLocationFormats = new string[0];
            viewEngine.AreaPartialViewLocationFormats = new string[0];
            viewEngine.AreaViewLocationFormats = new string[0];
            viewEngine.MasterLocationFormats = new string[0];

            var allPaths = extensions.Select(e => folderPath + "/{0}" + e).Concat(fallbackPaths.SelectMany(f => extensions.Select(e => f + "/{0}" + e))).ToArray();
            viewEngine.PartialViewLocationFormats = allPaths;
            viewEngine.ViewLocationFormats = allPaths;
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext.Controller.GetType() != controllerType)
                return new ViewEngineResult(new string[0]);

            return viewEngine.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext.Controller.GetType() != controllerType)
                return new ViewEngineResult(new string[0]);

            return viewEngine.FindView(controllerContext, viewName, masterName, useCache);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            if (controllerContext.Controller.GetType() != controllerType)
                return;

            viewEngine.ReleaseView(controllerContext, view);
        }
    }
}
