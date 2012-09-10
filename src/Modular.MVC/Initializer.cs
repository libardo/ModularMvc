using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Modular.Mvc;
using Modular.Mvc.Implementation;
using System.Web.Hosting;
using System.Diagnostics;

namespace Modular.Mvc
{
    public class Initializer
    {
        public static Initializer Default
        {
            get { return new Initializer().AddAssemblies(Assembly.GetCallingAssembly()); }
        }


        public Initializer()
        {
            this.Settings = new Settings();
        }


        public Settings Settings { get; set; }


        public void Register(RouteCollection routeCollection, ViewEngineCollection viewEngineCollection)
        {
            foreach (var assembly in Settings.Assemblies)
            {
                foreach (var namespaceGroup in assembly.GetTypes()
                    .Where(t => typeof(IController).IsAssignableFrom(t))
                    .GroupBy(t => t.Namespace))
                {
                    var controllers = namespaceGroup.ToList();

                    //if (Settings.ScanResourceViews)
                    //{
                    //    if (assembly.GetManifestResourceNames().Any(rn => rn.StartsWith(assembly.GetName().Name + "." + Settings.ModuleRootPath)))
                    //    {
                    //        if (!(HostingEnvironment.VirtualPathProvider is Implementation.AssemblyResourceProvider))
                    //        {
                    //            HostingEnvironment.RegisterVirtualPathProvider(new Implementation.AssemblyResourceProvider());
                    //        }
                    //    }
                    //}

                    if (controllers.Count == 1)
                    {
                        foreach (var controllerType in controllers)
                        {
                            // only 1 controller -> controller is accessed via folder path 
                            RegisterController(controllerType, routeCollection, viewEngineCollection);
                        }
                    }
                    else
                    {
                        var defaultCandidates = Defaults.DefaultControllerCandidates(assembly.GetName().Name + "." + Settings.ModuleRootPath.Trim('~', '.', '/'), namespaceGroup.Key);
                        var defaultController = Settings.FindDefaultController(controllers, defaultCandidates);
                        if (defaultController != null)
                        {
                            // default controller is accessed via folder path
                            RegisterController(defaultController, routeCollection, viewEngineCollection);

                            foreach (var controllerType in controllers.Except(new[] { defaultController }))
                                // other controllers are accssed via folder path + controller name
                                RegisterController(controllerType, routeCollection, viewEngineCollection, Defaults.GetControllerName(controllerType));
                        }
                        else
                        {
                            foreach (var controllerType in controllers)
                                // no default controller - controllers are distinguished via folder path + controller name
                                RegisterController(controllerType, routeCollection, viewEngineCollection, Defaults.GetControllerName(controllerType));
                        }
                    }
                }
            }
        }

        private void RegisterController(Type controllerType, RouteCollection routes, ViewEngineCollection viewEngines, string subpath = "")
        {
            if (!controllerType.Namespace.StartsWith(controllerType.Assembly.GetName().Name))
            {
                Trace.WriteLine("Controller " + controllerType + " does not map assembly name " + controllerType.Assembly.GetName().Name + ". The controller namespace must start with assembly name");
                return;
            }

            string folderPath = Settings.FolderPath(controllerType);

            if (!folderPath.StartsWith(Settings.ModuleRootPath))
            {
                Trace.WriteLine("Controller " + controllerType + " is not located below required module root " + Settings.ModuleRootPath);
                return;
            }

            string url = folderPath.Substring(Settings.ModuleRootPath.Length).Trim('/');

            if (Settings.IsHomeSubpath(url))
            {
                // the controller is in /Home subfolder which means we want to route to it from it's parent path
                var segments = url.Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                url = string.Join("/", segments.Take(segments.Length - 1));
            }

            if (!string.IsNullOrEmpty(subpath))
                // subpath - typically a controller name when >1 controllers - is appended to the path
                url = Defaults.Combine(url, subpath);

            url = Defaults.Combine(url, "{action}");

            url = Settings.AppendCustomRouteSegments(url, controllerType);

            AddRoutes(routes, controllerType, url);

            AddViewEngines(viewEngines, controllerType, folderPath, url);
        }

        private void AddRoutes(RouteCollection routes, Type controllerType, string url)
        {
            if (routes == null)
                return;

            string controllerName = Defaults.GetControllerName(controllerType);
            routes.Add(new ModularRoute(new Route(url,
                Settings.DefaultRouteValues(controllerType),
                Settings.RouteConstraints(controllerType),
                Settings.RouteDataTokens(controllerType),
                new MvcRouteHandler()), controllerName));
        }

        private void AddViewEngines(ViewEngineCollection viewEngines, Type controllerType, string folderPath, string url)
        {
            if (viewEngines == null)
                return;
            
            var sharedViewLocationPaths = Settings.SharedViewPaths(Settings.ModuleRootPath, folderPath);
            var viewEngine = Settings.ViewEngine();
            viewEngines.Add(new ModularViewEngine(viewEngine, controllerType, "~/" + folderPath, sharedViewLocationPaths, Settings.ViewEngineExtensions(viewEngine)));
        }

        public void Unregister(RouteCollection routes, ViewEngineCollection viewEngines)
        {
            if (routes != null)
            {
                for (int i = routes.Count - 1; i >= 0; i--)
                {
                    if (routes[i] is ModularRoute)
                        routes.RemoveAt(i);
                }
            }
            if (viewEngines != null)
            {
                for (int i = viewEngines.Count - 1; i >= 0; i--)
                {
                    if (viewEngines[i] is ModularViewEngine)
                        viewEngines.RemoveAt(i);
                }
            }
        }
    }
}