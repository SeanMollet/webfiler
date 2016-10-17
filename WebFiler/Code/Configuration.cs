using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using WebFiler.Resources;

namespace WebFiler
{
    public static class Configuration
    {
        /// <summary>
        /// Gets the root path.
        /// </summary>
        /// <value>The _root.</value>
        public static string _root
        {
            get
            {
                //Make sure the root path is a full one. Convert relative paths to physical
                string root = WebConfigurationManager.AppSettings.Get(Strings.Root);
                if (!System.IO.Path.IsPathRooted(root))
                {
                    root = HttpContext.Current.Server.MapPath(root);
                }

                return root;
            }
        }
    }
}