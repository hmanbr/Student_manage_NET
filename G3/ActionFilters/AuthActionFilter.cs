using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace G3.ActionFilters {
    public class AuthActionFilter : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            string? user = filterContext.HttpContext.Session.GetString("User");
            if (user == null) {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {
                    controller = "Auth",
                    action = "SignIn"
                }));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}

