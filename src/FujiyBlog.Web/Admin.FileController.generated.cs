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
    public partial class FileController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public FileController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected FileController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Upload() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Upload);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public FileController Actions { get { return MVC.Admin.File; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Admin";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "File";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "File";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Upload = "Upload";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants {
            public const string Upload = "Upload";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_FileController: FujiyBlog.Web.Areas.Admin.Controllers.FileController {
        public T4MVC_FileController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Upload(System.Web.HttpPostedFileBase uploadFile, string folderName) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Upload);
            callInfo.RouteValueDictionary.Add("uploadFile", uploadFile);
            callInfo.RouteValueDictionary.Add("folderName", folderName);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
