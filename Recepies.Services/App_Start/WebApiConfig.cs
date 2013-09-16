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
                routeTemplate: "api/recipe/{action}/{id}",
                defaults: new { controller = "recipes", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "CommentsApi",
                routeTemplate: "api/comment/{id}/{action}",
                defaults: new { controller = "comments", id = RouteParameter.Optional, action = RouteParameter.Optional }
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
