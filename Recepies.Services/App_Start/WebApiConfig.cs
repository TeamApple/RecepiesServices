using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Recepies.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "RecipeApi",
                routeTemplate: "api/recipe/{action}",
                defaults: new { controller = "recipes" }
            );

            config.Routes.MapHttpRoute(
                name: "CommentsApi",
                routeTemplate: "api/comment/{action}",
                defaults: new { controller = "comments" }
            );

            config.Routes.MapHttpRoute(
                name: "UserLoginApi",
                routeTemplate: "api/auth/token",
                defaults: new { controller = "users", action = "token" });

            config.Routes.MapHttpRoute(
                name: "UserApi",
                routeTemplate: "api/users/{action}",
                defaults: new { controller = "users" });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
