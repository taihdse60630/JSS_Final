using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchingSystem.Controllers
{
    public class MessageFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            filterContext.Controller.ViewBag.successmessage = filterContext.Controller.TempData["successmessage"];
            filterContext.Controller.ViewBag.warningmessage = filterContext.Controller.TempData["warningmessage"];
            filterContext.Controller.ViewBag.errormessage = filterContext.Controller.TempData["errormessage"];
        }
    }
}