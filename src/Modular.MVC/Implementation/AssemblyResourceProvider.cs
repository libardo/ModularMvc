//
// Credits: Kurt Harriger
// http://www.codeproject.com/Articles/15494/Load-WebForms-and-UserControls-from-Embedded-Resou
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Modular.Mvc.Implementation
{
    public class AssemblyResourceProvider : System.Web.Hosting.VirtualPathProvider
    {
        public AssemblyResourceProvider() { }
        private bool IsAppResourcePath(string virtualPath)
        {
            String checkPath = VirtualPathUtility.ToAppRelative(virtualPath);
            return checkPath.StartsWith("~/App_Resource/",
                   StringComparison.InvariantCultureIgnoreCase);
        }
        public override bool FileExists(string virtualPath)
        {
            return (IsAppResourcePath(virtualPath) ||
                    base.FileExists(virtualPath));
        }
        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsAppResourcePath(virtualPath))
                return new AssemblyResourceVirtualFile(virtualPath);
            else
                return base.GetFile(virtualPath);
        }
        public override System.Web.Caching.CacheDependency
               GetCacheDependency(string virtualPath,
               System.Collections.IEnumerable virtualPathDependencies,
               DateTime utcStart)
        {
            if (IsAppResourcePath(virtualPath))
                return null;
            else
                return base.GetCacheDependency(virtualPath,
                       virtualPathDependencies, utcStart);
        }
    }
    public class AssemblyResourceVirtualFile : VirtualFile
    {
        string path;
        public AssemblyResourceVirtualFile(string virtualPath)
            : base(virtualPath)
        {
            path = VirtualPathUtility.ToAppRelative(virtualPath);
        }
        public override System.IO.Stream Open()
        {
            string[] parts = path.Split('/');
            string assemblyName = parts[2];
            string resourceName = parts[3];
            Assembly assembly;

            if (assemblyName.EndsWith(".dll"))
            {
                assemblyName = Path.Combine(HttpRuntime.BinDirectory, assemblyName);
                assembly = System.Reflection.Assembly.LoadFile(assemblyName);
            }
            else
            {
                assembly = Assembly.Load(assemblyName);
            }

            if (assembly != null)
            {
                return assembly.GetManifestResourceStream(resourceName);
            }
            return null;
        }
    }
}
