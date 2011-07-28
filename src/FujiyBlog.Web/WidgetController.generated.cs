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
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace FujiyBlog.Web.Controllers {
    public partial class WidgetController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected WidgetController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Index() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Index);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Add() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Add);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Remove() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Remove);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Edit() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Edit);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Sort() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Sort);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public WidgetController Actions { get { return MVC.Widget; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Widget";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Index = "Index";
            public readonly string Add = "Add";
            public readonly string Remove = "Remove";
            public readonly string Edit = "Edit";
            public readonly string Sort = "Sort";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
            public readonly string Administration = "~/Views/Widget/Administration.cshtml";
            public readonly string Archive = "~/Views/Widget/Archive.cshtml";
            public readonly string ArchiveEdit = "~/Views/Widget/ArchiveEdit.cshtml";
            public readonly string Categories = "~/Views/Widget/Categories.cshtml";
            public readonly string CategoriesEdit = "~/Views/Widget/CategoriesEdit.cshtml";
            public readonly string Html = "~/Views/Widget/Html.cshtml";
            public readonly string HtmlEdit = "~/Views/Widget/HtmlEdit.cshtml";
            public readonly string Index = "~/Views/Widget/Index.cshtml";
            public readonly string TagCloud = "~/Views/Widget/TagCloud.cshtml";
            public readonly string TagCloudEdit = "~/Views/Widget/TagCloudEdit.cshtml";
            public readonly string Widget = "~/Views/Widget/Widget.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_WidgetController: FujiyBlog.Web.Controllers.WidgetController {
        public T4MVC_WidgetController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index(string zoneName) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Index);
            callInfo.RouteValueDictionary.Add("zoneName", zoneName);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Add(string zoneName, string widgetName) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Add);
            callInfo.RouteValueDictionary.Add("zoneName", zoneName);
            callInfo.RouteValueDictionary.Add("widgetName", widgetName);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Remove(int settingsId) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Remove);
            callInfo.RouteValueDictionary.Add("settingsId", settingsId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(int widgetSettingId) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Edit);
            callInfo.RouteValueDictionary.Add("widgetSettingId", widgetSettingId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(int widgetSettingId, string settings) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Edit);
            callInfo.RouteValueDictionary.Add("widgetSettingId", widgetSettingId);
            callInfo.RouteValueDictionary.Add("settings", settings);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Sort(string widgetsOrder) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Sort);
            callInfo.RouteValueDictionary.Add("widgetsOrder", widgetsOrder);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
