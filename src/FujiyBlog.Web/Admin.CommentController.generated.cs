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
namespace FujiyBlog.Web.Areas.Admin.Controllers {
    public partial class CommentController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected CommentController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ViewResult Index() {
            return new T4MVC_ViewResult(Area, Name, ActionNames.Index);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ViewResult Pending() {
            return new T4MVC_ViewResult(Area, Name, ActionNames.Pending);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ApproveSelected() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.ApproveSelected);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult DisapproveSelected() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.DisapproveSelected);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult DeleteSelected() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.DeleteSelected);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Edit() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Edit);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Delete() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Delete);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CommentController Actions { get { return MVC.Admin.Comment; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Admin";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Comment";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Index = "Index";
            public readonly string Pending = "Pending";
            public readonly string ApproveSelected = "ApproveSelected";
            public readonly string DisapproveSelected = "DisapproveSelected";
            public readonly string DeleteSelected = "DeleteSelected";
            public readonly string Edit = "Edit";
            public readonly string Delete = "Delete";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
            public readonly string _Layout = "~/Areas/Admin/Views/Comment/_Layout.cshtml";
            public readonly string Edit = "~/Areas/Admin/Views/Comment/Edit.cshtml";
            public readonly string Index = "~/Areas/Admin/Views/Comment/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_CommentController: FujiyBlog.Web.Areas.Admin.Controllers.CommentController {
        public T4MVC_CommentController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ViewResult Index(int? page) {
            var callInfo = new T4MVC_ViewResult(Area, Name, ActionNames.Index);
            callInfo.RouteValueDictionary.Add("page", page);
            return callInfo;
        }

        public override System.Web.Mvc.ViewResult Pending(int? page) {
            var callInfo = new T4MVC_ViewResult(Area, Name, ActionNames.Pending);
            callInfo.RouteValueDictionary.Add("page", page);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ApproveSelected(System.Collections.Generic.IEnumerable<int> selectedComments) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ApproveSelected);
            callInfo.RouteValueDictionary.Add("selectedComments", selectedComments);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult DisapproveSelected(System.Collections.Generic.IEnumerable<int> selectedComments) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.DisapproveSelected);
            callInfo.RouteValueDictionary.Add("selectedComments", selectedComments);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult DeleteSelected(System.Collections.Generic.IEnumerable<int> selectedComments) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.DeleteSelected);
            callInfo.RouteValueDictionary.Add("selectedComments", selectedComments);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(int id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Edit);
            callInfo.RouteValueDictionary.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(FujiyBlog.Web.Areas.Admin.ViewModels.AdminCommentSave input) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Edit);
            callInfo.RouteValueDictionary.Add("input", input);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Delete(int id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Delete);
            callInfo.RouteValueDictionary.Add("id", id);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
