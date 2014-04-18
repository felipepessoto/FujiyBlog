// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace FujiyBlog.Web.Controllers
{
    public partial class SocialController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SocialController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected SocialController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult LoginOpenId()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LoginOpenId);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SocialController Actions { get { return MVC.Social; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Social";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Social";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string LoginOpenId = "LoginOpenId";
            public readonly string Logout = "Logout";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string LoginOpenId = "LoginOpenId";
            public const string Logout = "Logout";
        }


        static readonly ActionParamsClass_LoginOpenId s_params_LoginOpenId = new ActionParamsClass_LoginOpenId();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_LoginOpenId LoginOpenIdParams { get { return s_params_LoginOpenId; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_LoginOpenId
        {
            public readonly string openIdIdentifier = "openIdIdentifier";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_SocialController : FujiyBlog.Web.Controllers.SocialController
    {
        public T4MVC_SocialController() : base(Dummy.Instance) { }

        [NonAction]
        partial void LoginOpenIdOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string openIdIdentifier);

        [NonAction]
        public override System.Web.Mvc.ActionResult LoginOpenId(string openIdIdentifier)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LoginOpenId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "openIdIdentifier", openIdIdentifier);
            LoginOpenIdOverride(callInfo, openIdIdentifier);
            return callInfo;
        }

        [NonAction]
        partial void LogoutOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Logout()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Logout);
            LogoutOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
