using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.SignalR;
namespace SignalRRealTimeSQL
{
    public class Global : System.Web.HttpApplication
    {
        string connString = ConfigurationManager.ConnectionStrings["_SDE"].ConnectionString;
        protected void Application_Start(object sender, EventArgs e)
        {
            SqlDependency.Start(connString);
            RouteTable.Routes.MapHubs();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            SqlDependency.Stop(connString);
        }
    }
}