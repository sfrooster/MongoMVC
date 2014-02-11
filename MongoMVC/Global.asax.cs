using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using MongoDB.Bson;

using MongoMVC.Models;

namespace MongoMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(ObjectId), new ObjectIdBinder());
        }

        private class Logger : StreamWriter
        {
            private Logger(string filename)
                : base(string.Format(@"{0}\App_Data\logs\{1}", AppDomain.CurrentDomain.BaseDirectory, filename), true)
            {

            }

            private static Logger _instance;
            private static Logger instance
            {
                get
                {
                    if (_instance == null) _instance = new Logger("routelog.txt");
                    return _instance;
                }
            }

            public static void log(string message)
            {
                instance.WriteLine(string.Format("{0:yyyyMMdd HH:mm:ss.fff} {1}", DateTime.Now, message));
                instance.Flush();
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpContextBase ctx = new HttpContextWrapper(Context);
            foreach (Route rte in RouteTable.Routes)
            {
                if (rte.GetRouteData(ctx) != null)
                {
                    if (rte.RouteHandler.GetType().Name == "MvcRouteHandler")
                    {
                        Logger.log(string.Format("Following route: {1} for request: {0}", Context.Request.Url, rte.Url));
                    }
                    else
                    {
                        Logger.log(string.Format("Ignoring via route: {1} for request: {0}", Context.Request.Url, rte.Url));
                    }
                    break;
                }
            }
        }
    }
}
