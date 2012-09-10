using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Modular.Mvc
{
    public static class Defaults
    {
        public static Initializer AddAssemblies(this Initializer starter, params Assembly[] assemblies)
        {
            starter.Settings.Assemblies = starter.Settings.Assemblies.Union(assemblies).ToArray();
            return starter;
        }

        public static Initializer AddCallingAssembly(this Initializer starter)
        {
            return starter.AddAssemblies(Assembly.GetCallingAssembly());
        }

        public static Initializer BelowPath(this Initializer starter, string path = "Modules")
        {
            starter.Settings.ModuleRootPath = path;
            return starter;
        }

        public static Initializer PromoteControllerInSubpath(this Initializer starter, string subpath = "Home")
        {
            starter.Settings.IsHomeSubpath = (url) => url.EndsWith(subpath);
            return starter;
        }

        public static Initializer Route<TController>(this Initializer starter, string suburl = "{id}", object defaults = null, object constraints = null)
            where TController : IController
        {
            var defaultSegments = starter.Settings.AppendCustomRouteSegments;
            var defaultValues = starter.Settings.DefaultRouteValues;
            var defaultConstraints = starter.Settings.RouteConstraints;

            starter.Settings.AppendCustomRouteSegments = (url, type) =>
            {
                if (type != typeof(TController))
                    return defaultSegments(url, type);
                
                return Defaults.Combine(url, suburl);
            };

            if (defaults != null)
            {
                starter.Settings.DefaultRouteValues = (type) =>
                {
                    var values = defaultValues(type);
                    if (type != typeof(TController))
                        return values;

                    foreach (var kvp in new RouteValueDictionary(defaults))
                    {
                        values[kvp.Key] = kvp.Value;
                    }
                    return values;
                };
            }
            if (constraints != null)
            {
                starter.Settings.RouteConstraints = (type) =>
                {
                    var values = defaultConstraints(type);
                    if (type != typeof(TController))
                        return values;

                    foreach (var kvp in new RouteValueDictionary(constraints))
                    {
                        values[kvp.Key] = kvp.Value;
                    }
                    return values;
                };
            }

            return starter;
        }

        public static Initializer Configure(this Initializer starter, Action<Settings> configuration)
        {
            configuration(starter.Settings);
            return starter;
        }

        public static IEnumerable<string> SharedPaths(string moduleRootPath, string path)
        {
            return GetIncrementalSegments(path).Select(f => "~/" + moduleRootPath + f + "Shared").Reverse().ToList();
        }

        public static IEnumerable<string> GetIncrementalSegments(string url)
        {
            yield return "/";

            url = url.Trim('/');
            if (string.IsNullOrEmpty(url))
            {
                yield break;
            }

            string previous = "";
            foreach (var segment in url.Split('/'))
            {
                previous = previous + "/" + segment;
                yield return previous + "/";
            }
        }

        public static RouteValueDictionary RouteDataTokens(Type controllerType)
        {
            return new RouteValueDictionary(new { namespaces = new[] { controllerType.Namespace } });
        }

        class ExistingActionConstraint : IRouteConstraint
        {
            private HashSet<string> actions;

            public ExistingActionConstraint(Type controllerType)
            {
                actions = new HashSet<string>(new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().Select(ad => ad.ActionName));
            }

            public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
            {
                return actions.Contains(values["action"]);
            }
        }

        public static RouteValueDictionary RouteConstraints(Type controllerType)
        {
            return new RouteValueDictionary(new { action = new ExistingActionConstraint(controllerType) });
        }

        public static RouteValueDictionary DefaultRouteValues(Type controllerType)
        {
            return new RouteValueDictionary(new { action = "Index", controller = GetControllerName(controllerType) });
        }

        public static VirtualPathProviderViewEngine ViewEngine()
        {
            return new RazorViewEngine();
        }

        public static IEnumerable<string> ViewEngineExtensions(VirtualPathProviderViewEngine viewEngine)
        {
            return viewEngine.ViewLocationFormats.Select(p => VirtualPathUtility.GetExtension(p)).Distinct().ToList();
        }

        public static string FolderPath(Type controllerType)
        {
            return controllerType.Namespace.Substring(controllerType.Assembly.GetName().Name.Length + 1).Replace('.', '/');
        }

        public static IEnumerable<string> DefaultControllerCandidates(string rootNamespace, string controllerNamespace)
        {
            var previous = "";
            return controllerNamespace.Substring(rootNamespace.Length).Trim('.').Split('.').Select(segment =>
            {
                previous += segment;
                return previous;
            }).ToList();
        }

        public static Type FindDefaultController(IEnumerable<Type> controllers, IEnumerable<string> defaultControllerNames)
        {
            return controllers.FirstOrDefault(c => defaultControllerNames.Any(dc => dc == c.Name || dc + "Controller" == c.Name));
        }

        public static bool IsHomeSubpath(string url)
        {
            return url.EndsWith("Home/") || url.EndsWith("Home");
        }

        public static string Combine(string path, string subpath)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(subpath))
                return path + subpath;
            if (path.EndsWith("/") && subpath.StartsWith("/"))
                return path + subpath.Substring(1);
            if (path.EndsWith("/") || subpath.StartsWith("/"))
                return path + subpath;
            return path + "/" + subpath;
        }

        public static string GetControllerName(Type controllerType)
        {
            if (controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                return controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length);
            else
                return controllerType.Name;
        }

        public static string AppendCustomRouteSegments(string url, Type controllerType)
        {
            return url;
        }
    }
}
