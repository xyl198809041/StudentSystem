using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DataSystem.DB;
using DataSystem;

namespace WebSystem
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //服务器初始化
            AppSet.IsWeb = true;
            var a= DataList.Current;

            //a.AddJCRule(new JCRule()
            //{
            //    Tilte="jcrule2",
            //});
            //a.AddRule(new Rule()
            //{
            //    JCRule = a.JCRules[0],
            //    Tilte="rule1",
            //    Group="ceshi",
            //    Point=0
            //});
            //a.AddStudentMsg(new StudentMsg()
            //{
            //    Student = a.Students[0],
            //    Rule = a.Rules[0]
            //}); a.AddStudentMsg(new StudentMsg()
            //{
            //    Student = a.Students[0],
            //    Rule = a.Rules[0]
            //}); a.AddStudentMsg(new StudentMsg()
            //{
            //    Student = a.Students[0],
            //    Rule = a.Rules[0]
            //}); a.AddStudentMsg(new StudentMsg()
            //{
            //    Student = a.Students[0],
            //    Rule = a.Rules[0]
            //});
        }
    }
}
