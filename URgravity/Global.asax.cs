using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;


using System.Data.Entity;
using URgravity.Models;
using URgravity.DAL;


namespace URgravity
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            IServiceLocator injector =
    new WindsorServiceLocator(
        new WindsorContainer(
            new XmlInterpreter(
                new ConfigResource("oauth.net.components"))));

            ServiceLocator.SetLocatorProvider(() => injector);

            Database.SetInitializer<UserContext>(new UserInitializer());

        }

        //Bruno : We need to initialization the Session
        void Session_Start(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Add("__MyAppSession", string.Empty);
        }


    }
}