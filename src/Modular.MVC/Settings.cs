using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Modular.Mvc
{
    public class Settings
    {
        public Settings()
        {
            ModuleRootPath = "Modules";
            ScanResourceViews = true;
            Assemblies = new Assembly[0];
            SharedViewPaths = Defaults.SharedPaths;
            DefaultRouteValues = Defaults.DefaultRouteValues;
            RouteDataTokens = Defaults.RouteDataTokens;
            RouteConstraints = Defaults.RouteConstraints;
            ViewEngine = Defaults.ViewEngine;
            ViewEngineExtensions = Defaults.ViewEngineExtensions;
            FolderPath = Defaults.FolderPath;
            FindDefaultController = Defaults.FindDefaultController;
            IsHomeSubpath = Defaults.IsHomeSubpath;
            AppendCustomRouteSegments = Defaults.AppendCustomRouteSegments;
        }

        public string ModuleRootPath { get; set; }
        public bool ScanResourceViews { get; set; }
        public Assembly[] Assemblies { get; set; }
        public Func<string, string, IEnumerable<string>> SharedViewPaths { get; set; }
        public Func<Type, RouteValueDictionary> DefaultRouteValues { get; set; }
        public Func<Type, RouteValueDictionary> RouteDataTokens { get; set; }
        public Func<Type, RouteValueDictionary> RouteConstraints { get; set; }
        public Func<VirtualPathProviderViewEngine> ViewEngine { get; set; }
        public Func<VirtualPathProviderViewEngine, IEnumerable<string>> ViewEngineExtensions { get; set; }
        public Func<Type, string> FolderPath { get; set; }
        public Func<IEnumerable<Type>, IEnumerable<string>, Type> FindDefaultController { get; set; }
        public Func<string, bool> IsHomeSubpath { get; set; }
        public Func<string, Type, string> AppendCustomRouteSegments { get; set; }
    }
}
